// Copyright (c) 2015, Kinvey, Inc. All rights reserved.
//
// This software is licensed to you under the Kinvey terms of service located at
// http://www.kinvey.com/terms-of-use. By downloading, accessing and/or using this
// software, you hereby accept such terms of service  (and any agreement referenced
// therein) and agree that you have read, understand and agree to be bound by such
// terms of service and are of legal age to agree to such terms with Kinvey.
//
// This software contains valuable confidential and proprietary information of
// KINVEY, INC and is subject to applicable licensing agreements.
// Unauthorized reproduction, transmission or distribution of this file and its
// contents is a violation of applicable laws.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Remotion.Linq.Parsing.Structure;

namespace KinveyXamarin
{
	/// <summary>
	/// Each DataStore in your application represents a collection on your backend. The <code>DataStore</code> class manages the access of data between the Kinvey backend and the app.
	/// The <code>DataStore</code> provides simple CRUD operations on data, as well as powerful querying and synchronization APIs.
	/// 
	/// </summary>
	public class DataStore<T> : KinveyQueryable<T>  where T:class
	{
		#region Member variables

		private String collectionName;

		//private Type typeof(T);

		private AbstractClient client;

		private ICache<T> cache = null;

		private ISyncQueue syncQueue = null;

		private DataStoreType storeType = DataStoreType.SYNC;

		private JObject customRequestProperties = new JObject();

		private NetworkFactory networkFactory;

		/// <summary>
		/// Represents the name of the collection.
		/// </summary>
		/// <value>The name of the collection.</value>
		public string CollectionName {
			get { return this.collectionName; }
			set { this.collectionName = value; }
		}

		/// <summary>
		/// Gets or sets the Kinvey client that is used for making data requests. 
		/// <seealso cref="KinveyXamarin.Client"/>
		/// </summary>
		/// <value>The Kinvey client.</value>
		public AbstractClient KinveyClient
		{
			get { return this.client; }
			set { this.client = value; }
		}

		/// <summary>
		/// Gets the custom request properties.
		/// </summary>
		/// <returns>The custom request properties.</returns>
		public JObject GetCustomRequestProperties(){
			return this.customRequestProperties;
		}

		/// <summary>
		/// Sets the custom request properties.
		/// </summary>
		/// <param name="customheaders">Customheaders.</param>
		public void SetCustomRequestProperties (JObject customheaders)
		{
			this.customRequestProperties = customheaders;
		}

		/// <summary>
		/// Sets a specific custom request property to a JSON object.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		public void SetCustomRequestProperty (string key, JObject value)
		{
			if (this.customRequestProperties == null) {
				this.customRequestProperties = new JObject ();
			}
			this.customRequestProperties.Add (key, value);
		}

		#endregion

		private DataStore (DataStoreType type, string collectionName, AbstractClient client = null)
			: base (new KinveyQueryProvider(typeof(KinveyQueryable<T>), QueryParser.CreateDefault(), new KinveyQueryExecutor<T>()), typeof(T))
		{
			this.collectionName = collectionName;

			if (client != null)
			{
				this.client = client;
			}
			else
			{
				this.client = Client.SharedClient;
			}

			this.cache = this.client.CacheManager.GetCache<T> (collectionName);
			this.syncQueue = this.client.CacheManager.GetSyncQueue (collectionName);
			this.storeType = type;
			this.customRequestProperties = this.client.GetCustomRequestProperties();
			this.networkFactory = new NetworkFactory(this.client);
		}

		#region Public interface

		/// <summary>
		/// Gets an instance of the <see cref="KinveyXamarin.DataStore{T}"/>.
		/// </summary>
		/// <returns>The DataStore instance.</returns>
		/// <param name="type">The <see cref="KinveyXamarin.DataStoreType"/> of this DataStore instance</param>
		/// <param name="collectionName">Collection name of the Kinvey collection backing this DataStore</param>
		/// <param name="client">Kinvey Client used by this DataStore (optional). If the client is not specified, the <code>Client.sharedClient</code> is used.</param>
		public static DataStore<T> Collection(string collectionName, DataStoreType type, AbstractClient client = null)
		{
			// TODO do we need to make this a singleton based on collection, store type and store ID?
			return new DataStore<T> (type, collectionName, client);
		}

//		/// <summary>
//		/// Get a single entity stored in a Kinvey collection.
//		/// </summary>
//		/// <returns>The async task.</returns>
//		/// <param name="entityId">Entity identifier.</param>
//		public async Task<T> FindByIDAsync(string entityID)
//		{
//			List<string> entityIDs = new List<string>();
//			entityIDs.Add(entityID);
//			FindRequest<T> findByIDsRequest = new FindRequest<T>(client, collectionName, cache, storeType.ReadPolicy, entityIDs, null);
//			List<T> listEntities = await findByIDsRequest.ExecuteAsync();
//			return listEntities.FirstOrDefault();
//		}

//		/// <summary>
//		/// Get a single entity stored in a Kinvey collection.
//		/// </summary>
//		/// <returns>The async task.</returns>
//		/// <param name="entityId">Entity identifier.</param>
//		internal async Task<List<T>> FindByIDsAsync(List<string> entityIDs)
//		{
//			FindRequest<T> findByIDsRequest = new FindRequest<T>(client, collectionName, cache, storeType.ReadPolicy, entityIDs, null);
//			return await findByIDsRequest.ExecuteAsync();
//		}

		internal async Task<List<T>> FindWithMongoQueryAsync(string queryString)
		{
			return await networkFactory.buildGetRequest<T>(this.CollectionName, queryString).ExecuteAsync();
		}

		/// <summary>
		/// Perfoms a find operation, with an optional query filter.
		/// </summary>
		/// <param name="query">[optional] LINQ-style query that can be used to filter the search results</param>
		/// <param name="cacheResults">[optional] The intermediate cache results, returned via delegate prior to the 
		/// network results being returned.  This is only valid if the <see cref="KinveyXamarin.DataStoreType"/> is 
		/// <see cref="KinveyXamarin.DataStoreType.CACHE"/></param>
		/// <param name="ct">[optional] CancellationToken used to cancel the request.</param>
		public async Task<List<T>> FindAsync(IQueryable<T> query = null, KinveyDelegate<List<T>> cacheResults = null, CancellationToken ct = default(CancellationToken))
		{
			FindRequest<T> findByQueryRequest = new FindRequest<T>(client, collectionName, cache, storeType.ReadPolicy, cacheResults, query, null);
			ct.ThrowIfCancellationRequested();
			return await findByQueryRequest.ExecuteAsync();
		}

		/// <summary>
		/// Perfoms a find operation, with an optional query filter.
		/// </summary>
		/// <param name="entityID">The ID of the entity to be retrieved</param>
		/// <param name="cacheResults">[optional] The intermediate cache results, returned via delegate prior to the 
		/// network results being returned.  This is only valid if the <see cref="KinveyXamarin.DataStoreType"/> is 
		/// <see cref="KinveyXamarin.DataStoreType.CACHE"/></param>
		/// <param name="ct">[optional] CancellationToken used to cancel the request.</param>
		public async Task<List<T>> FindAsync(string entityID, KinveyDelegate<List<T>> cacheResults = null, CancellationToken ct = default(CancellationToken))
		{
			List<string> listIDs = new List<string>();

			if (entityID != null)
			{
				listIDs.Add(entityID);
			}

			FindRequest<T> findByQueryRequest = new FindRequest<T>(client, collectionName, cache, storeType.ReadPolicy, cacheResults, null, listIDs);
			ct.ThrowIfCancellationRequested();
			return await findByQueryRequest.ExecuteAsync();
		}

		/// <summary>
		/// Gets a count of all the entities in a collection
		/// </summary>
		/// <returns>The async task which returns the count.</returns>
		/// <param name="ct">[optional] CancellationToken used to cancel the request.</param>
		public async Task<uint> GetCountAsync(IQueryable<T> query = null, KinveyDelegate<uint> cacheCount = null, CancellationToken ct = default(CancellationToken))
		{
			GetCountRequest<T> getCountRequest = new GetCountRequest<T>(client, collectionName, cache, storeType.ReadPolicy, cacheCount, query);
			ct.ThrowIfCancellationRequested();
			return await getCountRequest.ExecuteAsync();
		}

		/// <summary>
		/// Save the specified entity to a Kinvey collection.
		/// </summary>
		/// <returns>The async task.</returns>
		/// <param name="entity">the entity to save.</param>
		/// <param name="ct">[optional] CancellationToken used to cancel the request.</param>
		public async Task<T> SaveAsync(T entity, CancellationToken ct = default(CancellationToken))
		{
			SaveRequest<T> request = new SaveRequest<T>(entity, this.client, this.CollectionName, this.cache, this.syncQueue, this.storeType.WritePolicy);
			ct.ThrowIfCancellationRequested();
			return await request.ExecuteAsync();
		}


		/// <summary>
		/// Deletes the entity associated with the provided id
		/// </summary>
		/// <returns>The async task.</returns>
		/// <param name="entityID">The Kinvey ID of the entity to delete.</param>
		/// <param name="ct">[optional] CancellationToken used to cancel the request.</param>
		public async Task<KinveyDeleteResponse> RemoveAsync(string entityID, CancellationToken ct = default(CancellationToken))
		{
			RemoveRequest<T> request = new RemoveRequest<T>(entityID, client, CollectionName, cache, syncQueue, storeType.WritePolicy);
			ct.ThrowIfCancellationRequested();
			return await request.ExecuteAsync();
		}

		/// <summary>
		/// Pulls data from the backend to local storage
		///
		/// This API is not supported on a DataStore of type <code>DataStoreType.Network</code>. Calling <code>sync()</code> on a <code>Network</code> store will throw an exception.
		/// </summary>
		/// <returns>Entities that were pulled from the backend.</returns>
		/// <param name="query">Optional Query parameter.</param>
		/// <param name="ct">[optional] CancellationToken used to cancel the request.</param>
		public async Task<List<T>> PullAsync(IQueryable<T> query = null, CancellationToken ct = default(CancellationToken))
		{
			if (this.storeType == DataStoreType.NETWORK)
			{
				throw new KinveyException(EnumErrorCategory.ERROR_DATASTORE_NETWORK, EnumErrorCode.ERROR_DATASTORE_INVALID_PULL_OPERATION, "");
			}

			if (this.GetSyncCount () > 0)
			{
				throw new KinveyException(EnumErrorCategory.ERROR_DATASTORE_NETWORK, EnumErrorCode.ERROR_DATASTORE_PULL_ONLY_ON_CLEAN_SYNC_QUEUE, "");
			}

			//TODO query
			PullRequest<T> pullRequest = new PullRequest<T> (client, CollectionName, cache, query);
			ct.ThrowIfCancellationRequested();
			return await pullRequest.ExecuteAsync ();
		}

		/// <summary>
		/// Push local data in the datastore to the backend.
		/// This API is not supported on a DataStore of type <code>DataStoreType.Network</code>. Calling <code>sync()</code> on a <code>Network</code> store will throw an exception.
		/// </summary>
		/// <returns>DataStoreResponse indicating errors, if any.</returns>
		/// <param name="ct">[optional] CancellationToken used to cancel the request.</param>
		public async Task<DataStoreResponse> PushAsync(CancellationToken ct = default(CancellationToken))
		{
			if (this.storeType == DataStoreType.NETWORK)
			{
				throw new KinveyException(EnumErrorCategory.ERROR_DATASTORE_NETWORK, EnumErrorCode.ERROR_DATASTORE_INVALID_PUSH_OPERATION, "");
			}

			PushRequest<T> pushRequest = new PushRequest<T>(client, CollectionName, cache, syncQueue, storeType.WritePolicy);
			ct.ThrowIfCancellationRequested();
			return await pushRequest.ExecuteAsync();
		}

		/// <summary>
		/// Kicks off a bi-directional synchronization of data between the library and the backend. 
		/// First, the library calls push to send local changes to the backend. Subsequently, the library calls pull to fetch data in the collection from the backend and stores it on the device.
		/// You can provide a query as a parameter to the sync API, to restrict the data that is pulled from the backend. The query does not affect what data gets pushed to the backend.
		///
		/// This API is not supported on a DataStore of type <code>DataStoreType.Network</code>. Calling <code>sync()</code> on a <code>Network</code> store will throw an exception.
		/// </summary>
		/// <returns>DataStoreResponse indicating errors, if any.</returns>
		/// <param name="query">An optional query parameter that controls what gets pulled from the backend during a sync operation.</param>
		/// <param name="ct">[optional] CancellationToken used to cancel the request.</param>
		public async Task<DataStoreResponse> SyncAsync (IQueryable<T> query = null, CancellationToken ct = default(CancellationToken))
		{
			if (this.storeType == DataStoreType.NETWORK)
			{
				throw new KinveyException(EnumErrorCategory.ERROR_DATASTORE_NETWORK, EnumErrorCode.ERROR_DATASTORE_INVALID_SYNC_OPERATION, "");
			}

			// first push
			DataStoreResponse response = await this.PushAsync();   //partial success

			ct.ThrowIfCancellationRequested();

			//then pull
			try
			{
				await this.PullAsync();
			}
			catch (KinveyException e)
			{
				response.addKinveyException (e);
			}

			return response;
		}

		/// <summary>
		/// Get a count of the number of items currently in the sync queue, either for a particular collection or total count.
		/// </summary>
		/// <returns>The sync queue item count.</returns>
		/// <param name="allCollections">[optional] Flag to determine if count should be for all collections.  Default to false.</param>
		public int GetSyncCount(bool allCollections = false)
		{
			return syncQueue.Count(allCollections);
		}

		#endregion

		#region Requests

//		[JsonObject (MemberSerialization.OptIn)]
//		public abstract class GetListRequest<T>:AbstractDataRequest<List<T>>{
//			public ICache<T> Cache { get; set; }
//
//			public GetListRequest (AbstractClient client, string REST_PATH, string collection)
//				: base (client, "GET", REST_PATH, default(T[]), collection){
//
//			}
////			public async override Task<List<T>> ExecuteAsync(){
////				List<T> ret = await this.Cache.GetAsync ();
////				if (ret != null && ret.Count > 0) {
////					//cached data found
////					//return ret;
////					//Cool! 
////				} else {
////				}
////				ret = await base.ExecuteAsync ();
////				this.Cache.SaveAsync (ret);
////
////				return ret;
////			}
//
//		}
//		/// <summary>
//		/// A Get request, which is implemented synchronously
//		/// </summary>
//		[JsonObject (MemberSerialization.OptIn)]
//		public class GetRequest <T> : GetListRequest<T>
//		{
//			private const string REST_PATH = "appdata/{appKey}/{collectionName}/";
//
//			public GetRequest (AbstractClient client, string collection)
//				: base (client, REST_PATH, collection)
//			{
//			}
//
//		}
//
//		/// <summary>
//		/// Get entity request, which is implemented synchronously
//		/// </summary>
//		[JsonObject (MemberSerialization.OptIn)]
//		public class GetEntityRequest <T> : AbstractDataRequest<T>
//		{
//			public ICache<T> Cache { get; set; }
//
//			private const string REST_PATH = "appdata/{appKey}/{collectionName}/{entityId}";
//
//			[JsonProperty]
//			public string EntityId { get; set; }
//
//			public GetEntityRequest (string entityId, AbstractClient client, string collection)
//				: base (client, "GET", REST_PATH, default(T), collection)
//			{
//				this.EntityId = entityId;
//				uriResourceParameters.Add ("entityId", entityId);
//
//			}
//
//		}
//
//		/// <summary>
//		/// Get query request, which is implemented synchronously
//		/// </summary>
//		[JsonObject (MemberSerialization.OptIn)]
//		public class GetQueryRequest <T> : GetListRequest<T>
//		{
//			private const string REST_PATH = "appdata/{appKey}/{collectionName}/?query={querystring}";
//
//			[JsonProperty]
//			public string QueryString { get; set; }
//
//			public GetQueryRequest (string queryString, AbstractClient client, string collection)
//				: base (client, REST_PATH, collection)
//			{
//			
//				string queryBuilder = "query=" + queryString;
//			
//				var decodedQueryMap = queryBuilder.Split('&')
//						.ToDictionary(c => c.Split('=')[0],
//						c => Uri.UnescapeDataString(c.Split('=')[1]));
//			
//				if (decodedQueryMap.ContainsKey("skip")){
//					this.uriTemplate += "&skip={skip}";
//					this.uriResourceParameters.Add("skip", decodedQueryMap["skip"]);
//				}
//				if (decodedQueryMap.ContainsKey("limit")){
//					this.uriTemplate += "&limit={limit}";
//					this.uriResourceParameters.Add("limit", decodedQueryMap["limit"]);		
//				}
//
//				if (decodedQueryMap.ContainsKey("sort")) {
//					this.uriTemplate += "&sort={sort}";
//					this.uriResourceParameters.Add("sort", decodedQueryMap["sort"]);
//				}
//
//				this.QueryString = decodedQueryMap["query"];
//				this.uriResourceParameters["querystring"] = this.QueryString;
//
//			}
//
//
//		}
//
//		/// <summary>
//		/// Get the count request, which is implemented synchronously.
//		/// </summary>
//		[JsonObject (MemberSerialization.OptIn)]
//		public class GetCountRequest : AbstractDataRequest<JObject>
//		{
//			private const string REST_PATH = "appdata/{appKey}/{collectionName}/_count";
//
//			public GetCountRequest(AbstractClient client, string collection)
//				: base(client, "GET", REST_PATH, default(JObject), collection)
//			{
//			}
//		}
//
//		/// <summary>
//		/// Get the count request, which is implemented synchronously.
//		/// </summary>
//		[JsonObject (MemberSerialization.OptIn)]
//		public class GetCountQueryRequest : AbstractDataRequest<JObject>
//		{
//			private const string REST_PATH = "appdata/{appKey}/{collectionName}/_count?query={querystring}";
//
//			[JsonProperty]
//			public string QueryString { get; set; }
//
//			public GetCountQueryRequest(string queryString, AbstractClient client, string collection)
//				: base(client, "GET", REST_PATH, default(JObject), collection)
//			{
//				string queryBuilder = "query=" + queryString;
//
//				var decodedQueryMap = queryBuilder.Split('&')
//					.ToDictionary(c => c.Split('=')[0],
//						c => Uri.UnescapeDataString(c.Split('=')[1]));
//
//				if (decodedQueryMap.ContainsKey("skip")){
//					this.uriTemplate += "&skip={skip}";
//					this.uriResourceParameters.Add("skip", decodedQueryMap["skip"]);
//				}
//				if (decodedQueryMap.ContainsKey("limit")){
//					this.uriTemplate += "&limit={limit}";
//					this.uriResourceParameters.Add("limit", decodedQueryMap["limit"]);
//				}
//
//				if (decodedQueryMap.ContainsKey("sort")) {
//					this.uriTemplate += "&sort={sort}";
//					this.uriResourceParameters.Add("sort", decodedQueryMap["sort"]);
//				}
//
//				this.QueryString = decodedQueryMap["query"];
//				this.uriResourceParameters["querystring"] = this.QueryString;
//			}
//		}
//			
//		/// <summary>
//		/// Delete request, which is implemented synchronously.
//		/// </summary>
//		[JsonObject (MemberSerialization.OptIn)]
//		public class DeleteRequest : AbstractDataRequest<KinveyDeleteResponse>
//		{
//
//			private const string REST_PATH = "appdata/{appKey}/{collectionName}/{entityId}";
//
//			[JsonProperty]
//			public string EntityId { get; set; }
//
//			public DeleteRequest (string entityId, AbstractClient client, string collectionName)
//				: base (client, "DELETE", REST_PATH, default(KinveyDeleteResponse), collectionName)
//			{
//				this.EntityId = entityId;
//				uriResourceParameters.Add ("entityId", entityId);
//			}
//
//		}			

		#endregion
	}
}
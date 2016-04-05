﻿// Copyright (c) 2015, Kinvey, Inc. All rights reserved.
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
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite.Net.Async;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KinveyXamarin
{
	/// <summary>
	/// This is an implementation of an OfflineStore, using SQLite to manage maintaining data.
	/// This class is responsible for breaking apart a request, and determing what actions to take
	/// Actual actions are performed on the OfflineTable class, using a SQLiteDatabaseHelper
	/// </summary>
	public class SQLiteCache <T> : ICache <T>
	{

		private string collectionName;

		/// <summary>
		/// The db connection.
		/// </summary>
		private SQLiteAsyncConnection dbConnection;

		/// <summary>
		/// Initializes a new instance of the <see cref="KinveyXamarin.SQLiteOfflineStore"/> class.
		/// </summary>
		/// <param name="collection">Collection.</param>
		/// <param name="connection">Connection.</param>
		public SQLiteCache(string collection, SQLiteAsyncConnection connection){
			this.collectionName = collection;
			this.dbConnection = connection;
			createTableAsync ();
		}


		/// <summary>
		/// Creates an Offline Table, which manages all offline collection features.
		/// </summary>
		public async Task<int> createTableAsync ()
		{
			await onCreateAsync ();
			return 0;
		}

		private async Task<int> onCreateAsync(){
			await dbConnection.CreateTableAsync<SQLTemplates.TableItem> ();
			await dbConnection.CreateTableAsync<SQLTemplates.QueueItem> ();
			await dbConnection.CreateTableAsync<SQLTemplates.QueryItem> ();
			await dbConnection.CreateTableAsync<SQLTemplates.OfflineEntity> ();


			//create the collection item and store it in the collection list
			SQLTemplates.TableItem table = new SQLTemplates.TableItem ();
			table.name = this.collectionName;
			await dbConnection.InsertAsync(table);

			return 0;
		}


		private async Task<int> deleteContentsOfTableAsync ()
		{
			int result = await dbConnection.DropTableAsync<SQLTemplates.TableItem> ();
			return result;

		}


		public async Task<List<T>> GetAsync (string query){
			return null;
		}

		public async Task<T> GetByIdAsync (string id){
			return default(T);
		}

		public async Task<List<T>> GetAsync (List<string> ids){
			return default(List<T>);
		}

		public async Task<List<T>> GetAsync (){
			return default(List<T>);
		}

		public async Task<List<T>> SaveAsync (List<T> items){
			return default(List<T>);
		}

		public async Task<T> SaveAsync (T item){
			return default(T);
		}

		public async Task<KinveyDeleteResponse> DeleteAsync (string query){
			return null;
		}

		public async Task<KinveyDeleteResponse> DeleteByIdAsync (string id){
			return null;
		}

		public async Task<KinveyDeleteResponse> DeleteAsync (List<string> ids){
			return null;
		}


//		public async Task<object> GetAsync(AbstractKinveyOfflineClientRequest<T> request){
//
//			//expand the URL
//			string targetURI = request.uriTemplate;
//			foreach (var p in request.uriResourceParameters)
//			{
//				targetURI = targetURI.Replace("{" + p.Key + "}", p.Value.ToString());
//			}
//
//			int idIndex = targetURI.IndexOf(this.collectionName) + this.collectionName.Length + 1;
//
//
//
//			object ret = null;
//			//is it a query?  (12 is magic number for decoding empty query string)
//			if (targetURI.Contains ("query") && (targetURI.IndexOf ("query") + 12) != targetURI.Length) {
//			
//				//it's a query!
//				//pull the actual query string out and get rid of the "?query"
//				String query = targetURI.Substring(idIndex, targetURI.Length - idIndex);
//				query = query.Replace("?query=","");
//				query = WebUtility.UrlDecode(query);
//
//				await createTableAsync ();
//
//				T[] ok = await getQueryAsync(query);
//
//
//				await enqueueRequestAsync("QUERY", query, request);
//				return ok;
//
//			} else if (idIndex == (targetURI.Length + 1)|| targetURI.Contains ("query")) {
//				//it's a get all request (no query, no id)
//				await createTableAsync ();
//				List<T> ok = await getAllAsync ();
//				return ok;
//			} else {
//				//it's a get by id
//				String targetID = targetURI.Substring(idIndex, targetURI.Length - idIndex);
//				await createTableAsync ();
//				ret = (T) await getEntityAsync (targetID);
//
//				await enqueueRequestAsync("GET", targetURI.Substring(idIndex, targetURI.Length - idIndex), request);
//			}
//
//
//			return ret;
//		}
//
//		private async Task<T[]> getQueryAsync (string queryString)
//		{
//			
//			SQLTemplates.QueryItem query = await dbConnection.Table<SQLTemplates.QueryItem>().Where(t => t.query == queryString && t.collection == collectionName).FirstOrDefaultAsync();
//
//			if (query == null) {
//				return null;
//			}
//
//			List<SQLTemplates.OfflineEntity> entities = new List<SQLTemplates.OfflineEntity>();
//
//			string[] ids = query.commaDelimitedIds.Split (',');
//
//			foreach (string id in ids){
//				entities.Add(dbConnection.Table<SQLTemplates.OfflineEntity>().Where(t => t.id == id && t.collection == collectionName).FirstOrDefaultAsync().Result);
//			}
//
//			T[] results = new T[ids.Length];
//
//			for (int i = 0; i < results.Length; i++){
//				results[i] = JsonConvert.DeserializeObject<T>(entities[i].json);
//			}
//
//			return results;
//
//		}
//
//
//		public async Task<object> SaveAsync (AbstractKinveyOfflineClientRequest<T> request){
//			//DatabaseHelper<T> handler = SQLiteCacheManager.getDatabaseHelper<T> ();
//
//			//grab json content and put it in the store
//			string jsonContent = null;
//			if (request.HttpContent != null) {
//				jsonContent = JsonConvert.SerializeObject (request.HttpContent);
//			}
//		
//
//			//grab the ID
//			JToken token = JObject.Parse(jsonContent);
//			string id = (string)token.SelectToken("_id");
//
//			//insert the entity into the database
//			await createTableAsync ();
//			await upsertEntityAsync(id, jsonContent);
//			//enque the request
//			await enqueueRequestAsync("PUT", id, request);
//
//			return request.HttpContent;
//		}
//
//
//		public async Task<KinveyDeleteResponse> DeleteAsync (AbstractKinveyOfflineClientRequest<T> request){
//		
//		}
//
//		public async Task<int> saveQueryResultsAsync (string queryString, string collection, List<string> ids)
//		{
//			SQLTemplates.QueryItem query = new SQLTemplates.QueryItem ();
//			query.query = queryString;
//			query.collection = collection;
//			query.commaDelimitedIds = String.Join (",", ids); 
//
//
//			int count = await dbConnection.UpdateAsync (query);
//			if (count == 0) {
//				await dbConnection.InsertAsync (query);
//			}
//
//			return 0;
//		}
//
//
//		public async Task<int> enqueueRequestAsync (string action, string id, AbstractKinveyOfflineClientRequest<T> req)
//		{
//			SQLTemplates.QueueItem queue = new SQLTemplates.QueueItem ();
//			queue.action = action;
//			queue.collection = collectionName;
//
//			SQLTemplates.OfflineMetaData metadata = new SQLTemplates.OfflineMetaData ();
//			metadata.id = id;
//			metadata.customHeaders = req.customRequestHeaders;
//
//			queue.OfflineMetaDataAsJson = JsonConvert.SerializeObject (metadata);
//			//queue.id = metadata;
//
//			await dbConnection.InsertAsync (queue);
//
//			return 0;
//		}
//
//		public async Task<int> enqueueRequestAsync (string action,SQLTemplates.OfflineMetaData metadata)
//		{
//			SQLTemplates.QueueItem queue = new SQLTemplates.QueueItem ();
//			queue.action = action;
//			queue.collection = collectionName;
//
//			queue.OfflineMetaDataAsJson = JsonConvert.SerializeObject (metadata);
//
//			await dbConnection.InsertAsync (queue);
//
//			return 0;
//		}
//
//		public async Task<List<T>> getAllAsync ()
//		{
//
//			List<SQLTemplates.OfflineEntity> entities = await dbConnection.Table<SQLTemplates.OfflineEntity> ().Where (t => t.collection == collectionName).ToListAsync ();
//
//			List<T> results = new List<T>();
//
//			foreach (SQLTemplates.OfflineEntity ent in entities) {
//				results.Add(JsonConvert.DeserializeObject<T>(ent.json));
//			}
//
//			return results;
//		}
//
//
//		public async Task<T> getEntityAsync (string id)
//		{
//
//			SQLTemplates.OfflineEntity entity = await dbConnection.Table<SQLTemplates.OfflineEntity> ().Where (t => t.collection == collectionName && t.id == id).FirstOrDefaultAsync ();
//
//			if (entity == default(SQLTemplates.OfflineEntity)) {
//				return default(T);
//			}
//			return JsonConvert.DeserializeObject<T> (entity.json);
//
//		}
//
//		public async Task<KinveyDeleteResponse> deleteAsync(string id)
//		{
//
//			SQLTemplates.OfflineEntity entity = await dbConnection.Table<SQLTemplates.OfflineEntity> ().Where (t => t.collection == collectionName && t.id == id).FirstOrDefaultAsync ();
//
//			int count = await dbConnection.DeleteAsync (entity.id);
//
//
//			KinveyDeleteResponse resp = new KinveyDeleteResponse ();
//			resp.count = count;
//
//			return resp;
//		}
//
//		public async Task<SQLTemplates.QueueItem> popQueueAsync (){
//			SQLTemplates.QueueItem item = await dbConnection.Table<SQLTemplates.QueueItem> ().FirstOrDefaultAsync ();
//			await removeFromQueueAsync (item.key);
//			return item;
//
//		}
//
//		public async Task<int> removeFromQueueAsync (int primaryKey)
//		{
//
//			await dbConnection.DeleteAsync<SQLTemplates.QueueItem> (primaryKey);
//			return 1;
//
//		}
//
//		/// <summary>
//		/// Executes a delete request.
//		/// </summary>
//		/// <returns>The delete.</returns>
//		/// <param name="client">Client.</param>
//		/// <param name="appdata">Appdata.</param>
//		/// <param name="request">Request.</param>
//		/// <typeparam name="T">The type of the response.</typeparam>
//		/// <param name="appData">App data.</param>
//		public async Task<KinveyDeleteResponse> executeDeleteAsync<T>(AbstractKinveyClient client, AbstractKinveyOfflineClientRequest<T> request){
//			//DatabaseHelper<T> handler = SQLiteCacheManager.getDatabaseHelper<T> ();
//
//			//expand the URL
//			string targetURI = request.uriTemplate;
//			foreach (var p in request.uriResourceParameters)
//			{
//				targetURI = targetURI.Replace("{" + p.Key + "}", p.Value.ToString());
//			}
//			int idIndex = targetURI.IndexOf(this.collectionName) + this.collectionName.Length + 1;
//
//			String targetID = targetURI.Substring(idIndex, targetURI.Length - idIndex);
//
//
//
//
//			await createTableAsync ();
//			KinveyDeleteResponse ret = await deleteAsync(targetID);
//
//			await enqueueRequestAsync("DELETE", targetURI.Substring(idIndex, targetURI.Length - idIndex), request);
//			return ret;
//		}
//
//		/// <summary>
//		/// Inserts an entity directly into the database.
//		/// </summary>
//		/// <param name="client">Client.</param>
//		/// <param name="appdata">Appdata.</param>
//		/// <param name="entity">Entity.</param>
//		/// <typeparam name="T">The type of the response.</typeparam>
//		/// <param name="appData">App data.</param>
//		public async Task<int> insertEntityAsync<T>(AbstractKinveyClient client, T entity){
//
//			//DatabaseHelper<T> handler = SQLiteCacheManager.getDatabaseHelper<T> ();
//
//			string jsonContent = JsonConvert.SerializeObject (entity);
//
//			//grab the ID
//			JToken token = JObject.Parse(jsonContent);
//			string id = (string)token.SelectToken("_id");
//
//			await createTableAsync ();
//
//			await upsertEntityAsync( id, jsonContent);
//			return 0;
//
//		}
//
//		/// <summary>
//		/// Upserts a specific entity, adding it directly to to the offline table.
//		/// </summary>
//		/// <param name="id">Identifier.</param>
//		/// <param name="collection">Collection.</param>
//		/// <param name="json">Json.</param>
//		public async Task<T> upsertEntityAsync(string id, string json){
//			SQLTemplates.OfflineEntity entity = new SQLTemplates.OfflineEntity ();
//			entity.id = id;
//			entity.json = json;
//			entity.collection = collectionName;
//
//
//			int count = await dbConnection.UpdateAsync (entity);
//			if (count == 0) {
//				await dbConnection.InsertAsync (entity);
//			}
//
//			return JsonConvert.DeserializeObject<T> (json);
//
//		}
//
//

	}

}

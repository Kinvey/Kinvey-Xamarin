﻿using Foundation;
using UIKit;

using KinveyXamarin;
using SQLite.Net.Platform.XamarinIOS;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace testiosapp
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations

		public override UIWindow Window {
			get;
			set;
		}

		Client myClient;

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method

			myClient = new Client.Builder ("kid_byWWRXzJCe", "4a58018febe945fea5ba76c08ce1e870")
				.setFilePath(NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User) [0].ToString())
				.setOfflinePlatform(new SQLitePlatformIOS())
				.setLogger(delegate(string msg) { Console.WriteLine(msg);})
				.build ();
			Login ();

			return true;
		}

		private async Task<User> Login()
		{
//			Dictionary<string, JToken> attr = new Dictionary<string, JToken> ();
//			email.Add ("email", "gob@bluth.com");
//			attr.Add ("my_field", "blah blah");
//			var newuser = await myClient.User ().CreateAsync ("George Michael Bluth", "cousin", attr);
//			var newuser2 = await myClient.User ().CreateAsync ("Tobias Funke", "actor");
//			Dictionary<string, JToken> last_name = new Dictionary<string, JToken>();
//			last_name.Add("last_name", "Bluth");
//			await myClient.User ().CreateAsync ("Lindsay Bluth", "me", last_name);
//			await myClient.User ().CreateAsync ("Maeby Bluth", "Surely", last_name);

			User user = new User();
			try
			{
				user = await myClient.User ().LoginAsync ("test", "test");

			string str = "Finished Launching.";
			Console.WriteLine("VRG : " + str);
			Console.WriteLine("VRG: Logged in as: " + myClient.User().Id);

//			// test GetCount(query)
//			Console.WriteLine("VRG: Start GetCount(query) test.");
//
////			AsyncAppData<JObject> myWorkOrders = myClient.AppData<JObject> ("workOrderCollection", typeof(JObject));
//////			uint myCount = await myWorkOrders.GetCountAsync("{\"Notes\":\"Home of the Patriots!\"}");
////			uint myCount = await myWorkOrders.GetCountAsync();
////			Console.WriteLine("VRG: Work Order count is: " + myCount.ToString ());
//
////			myClient.AppData<JObject>("workOrderCollection", typeof(JObject)).GetCount(new KinveyDelegate<uint>()
//			myClient.AppData<JObject>("workOrderCollection", typeof(JObject)).GetCount("{\"City\":\"Boston\"}&sort={\"Address\":-1}", new KinveyDelegate<uint>()
//			{
//					onSuccess = (result) =>
//					{
//						Console.WriteLine("VRG(delegate): GetCount() returned: " + result.ToString());
//					},
//					onError = (error) =>
//					{
//						Console.WriteLine("VRG(delegate): GetCount() call failed");
//					}
//			});
//
//			Console.WriteLine("VRG: End GetCount(query) test.");

			// test Get(query)
			Console.WriteLine("VRG: Start Get(Query) test.");
			AsyncAppData<WorkOrder> myWorkOrders = myClient.AppData<WorkOrder> ("workOrderCollectio", typeof(WorkOrder));
			WorkOrder[] arrWO = await myWorkOrders.GetAsync();
			foreach (var wo in arrWO)
			{
				Console.WriteLine("VRG: WorkOrder Address -> " + wo.Address);
			}
//			WorkOrder[] arrWO = await myWorkOrders.GetAsync("{\"City\":\"Boston\"}&sort={\"Address\":-1}");
//			WorkOrder[] arrWO = await myWorkOrders.GetAsync("{\"City\":\"Boston\"}");
//			var query = from work in arrWO
//				orderby work.Address
//			            select work;
//			Task.Run ( () => {
//				foreach (WorkOrder wo in query) {
//					Console.WriteLine("VRG: (Linq sort) work order address -> " + wo.Address);
//				}
//			});
			Console.WriteLine ("VRG: End Get(Query) test.");

//			// test LINQ querying
//			Console.WriteLine("VRG: Start LINQ Querying test.");
//
//			AsyncAppData<WorkOrder> myWorkOrders = myClient.AppData<WorkOrder>("workOrderCollection", typeof(WorkOrder));
////			WorkOrder[] arrWO = await myWorkOrders.GetAsync();
////			var query =quick from work in arrWO
//			var query = from work in myWorkOrders
////					where work.City.StartsWith("Bos")
//					where work.Notes.Equals("Hom\"e of the Celtics!")
//		            select work;
//			Task.Run( () => {
//				try
//				{
//					foreach (WorkOrder wo in query)
//					{
//						Console.WriteLine("VRG (GetAsync) work order -> " + wo.Notes);
//					}
//				}
//				catch
//				{
//					Console.WriteLine("VRG error in query.");
//				}
//			});
//
////			var query = from work in myWorkOrders
////						//orderby work.Notes
////						select work;
////
////			Task.Run ( () => {
////				try
////				{
//// 					foreach (WorkOrder w in query)
////					{
////						Console.WriteLine("VRG: workOrder -> " + w.Address);
////					}
////				}
////				catch (Exception e)
////				{
////					Console.WriteLine("VRG error in query.");
////				}
////			});
//
//
//
//			Console.WriteLine("VRG: End LINQ Querying test.");

//			// test Lookup
//			Console.WriteLine("VRG: Start Lookup() test.");
//
//			UserDiscovery criteria = new UserDiscovery();
//			criteria.LastName = "Bluth";
//			criteria.Email = "gob@bluth.com";
//
////			// async lookup
////			User[] lookupUsers = await myClient.User().LookupAsync(criteria);
////			Console.WriteLine("VRG: Finished Lookup() call.");
////			if (lookupUsers == null)
////				Console.WriteLine ("VRG: LookupUsers is NULL");
////			else {
////				Console.WriteLine("VRG: Length of lookup array is " + lookupUsers.Length);
////				foreach (User u in lookupUsers) {
////					Console.WriteLine ("VRG: User -> " + u.UserName);
////				}
////			}
//
//			// lookup
//			myClient.User().Lookup(criteria, new KinveyDelegate<User[]>()
//			{
//					onSuccess = (result) =>
//					{
//						Console.WriteLine("VRG(delegate): Finished Lookup() call.");
//						if (result == null)
//							Console.WriteLine ("VRG(delegate): LookupUsers is NULL");
//						else {
//							Console.WriteLine("VRG(delegate): Length of lookup array is " + result.Length);
//							foreach (User u in result) {
//								Console.WriteLine ("VRG(delegate): User -> " + u.UserName);
//							}
//						}
//					},
//					onError = (error) =>
//					{
//						Console.WriteLine("VRG: Lookup call failed");
//					}
//			});
//
//			Console.WriteLine("VRG: End Lookup() test.");


//			// test Upload file
//			Console.WriteLine("VRG: Starting Upload test...");
//
////			User u = await myClient.User().RetrieveAsync("56c231ab1db6dc745200fb3a");
//			User me = myClient.CurrentUser;
//			Dictionary<string, JToken> myAttr = me.Attributes;
//
//			byte[] file = System.IO.File.ReadAllBytes("/Users/vinay/Engineering/Kinvey-Xamarin/test-ios-app/vinay.jpg");
//			int size = (file.Length+1024) * sizeof(byte);
//			Console.WriteLine ("VRG: completed file read");
//			FileMetaData md = new FileMetaData();
//			md.customMetadata["testkey"] = "value";
//			myClient.File().upload(md, file, new KinveyDelegate<FileMetaData>() {
//				onSuccess = (result) => {
//					Console.WriteLine("VRG (delegate) File upload successful!");
//				},
//
//				onError = (error) => {
//					Console.WriteLine("VRG (delegate) Error with File upload. :(");
//				}
//			});
//			Console.WriteLine("VRG: Finished Upload test.");


//			Console.WriteLine("VRG: Starting Download test...");
//			byte[] fileDL = new byte[size];
//			FileMetaData metaDL = new FileMetaData ();
//			metaDL.id = "c7bc9591-8335-4b21-84c5-94c4d3ec68fb";
//			metaDL.fileName = "test.jpg";
//			myClient.File().download(metaDL, fileDL, new KinveyDelegate<FileMetaData>());
//			Console.WriteLine ("VRG: Finished download of file, about to save...");
//			System.IO.File.WriteAllBytes ("/Users/vinay/Engineering/Kinvey-Xamarin/test-ios-app/profileDLFromAndroid.jpg", fileDL);
//			Console.WriteLine("VRG: Finished Download test.");
//
//			Console.WriteLine("VRG: Starting Download test...");
//			byte[] fileDL = new byte[22003];
//			string fileID = "b6fadc6d-b084-4513-8e8d-3d64d75f2164";
//			myClient.File().downloadMetadata(fileID, new KinveyDelegate<FileMetaData>()
//				{
//					onSuccess = (result) =>
//					{
//						Console.WriteLine("VRG: Download metadata success.");
////						var myValue = result.customFieldsAndValues["sonumber"];
////						Console.WriteLine("VRG: Value(sonumber) -> " + myValue.ToString());
//						foreach (KeyValuePair<string, JToken> entry in result.customFieldsAndValues)
//						{
//							Console.WriteLine("VRG: [" + entry.Key + "] -> [" + entry.Value + "]");
//						}
//
////						Console.WriteLine ("VRG: Finished download of file, about to save...");
////						try {
////							System.IO.File.WriteAllBytes ("/Users/vinay/Engineering/Kinvey-Xamarin/test-ios-app/testDL.jpg", fileDL);
////						} catch (Exception e) {
////							Console.WriteLine("VRG: Error saving file. -> " + e.StackTrace);
////						}
////						Console.WriteLine("VRG: Finished Download test.");
//					},
//					onError = (error) =>
//					{
//						Console.WriteLine("VRG: Download error.");
//					}
//				}
//			);
//
//			Console.WriteLine ("VRG: start file R/W test.");
//			byte[] testfile = System.IO.File.ReadAllBytes ("/Users/vinay/Desktop/vinay.jpg");
//			System.IO.File.WriteAllBytes ("/Users/vinay/Desktop/vinay2.jpg", testfile);
//			Console.WriteLine ("VRG: end file R/W test.");

//			// test GetByID
//			AsyncAppData<TestEntity> entityCollection = myClient.AppData<TestEntity>("testcollection", typeof(TestEntity));
//
//			TestEntity entity = await entityCollection.GetEntityAsync("56c374f4936ee1cf6a000d1e");
//			Console.WriteLine ("VRG: GetEntityById ID: " + entity.ID);
//			Console.WriteLine ("VRG: GetEntityById firstName: " + entity.firstName);
//			Console.WriteLine ("VRG: GetEntityById lastName: " + entity.lastName);

//			TestEntity ent = new TestEntity();
//			ent.firstName = "Bob";
//			ent.lastName = "Loblaw";
////			ent.ID = stableId;
//
//			TestEntity entitySave = await entityCollection.SaveAsync(ent);
//
//			Console.WriteLine("VRG saved entity: " + entitySave.ID);

//			await entityCollection.DeleteAsync("56c398420c0dec9671004235");
//			Console.WriteLine ("VRG: After DeleteAsync");

			}
			catch (KinveyJsonResponseException e)
			{
				Console.WriteLine("VRG (exception caught) Exception Request ID -> " + e.Details.RequestID);
				Console.WriteLine("VRG (exception caught) Exception Description -> " + e.Details.Description);
				Console.WriteLine("VRG (exception caught) Exception Error -> " + e.Details.Error);
				Console.WriteLine("VRG (exception caught) Exception Debug -> " + e.Details.Debug);
			}
			catch (KinveyException e)
			{
				Console.WriteLine("VRG (exception caught) Exception Request ID -> " + e.RequestID);
				Console.WriteLine("VRG (exception caught) Exception Reason -> " + e.Reason);
				Console.WriteLine("VRG (exception caught) Exception Explanation -> " + e.Explanation);
				Console.WriteLine("VRG (exception caught) Exception Fix -> " + e.Fix);
			}
			return user;
		}

		public override void OnResignActivation (UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.
		}

		public override void DidEnterBackground (UIApplication application)
		{
			// Use this method to release shared resources, save user data, invalidate timers and store the application state.
			// If your application supports background exection this method is called instead of WillTerminate when the user quits.
		}

		public override void WillEnterForeground (UIApplication application)
		{
			// Called as part of the transiton from background to active state.
			// Here you can undo many of the changes made on entering the background.
		}

		public override void OnActivated (UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.
		}

		public override void WillTerminate (UIApplication application)
		{
			// Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
		}
	}
}


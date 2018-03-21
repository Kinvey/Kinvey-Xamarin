﻿// Copyright (c) 2016, Kinvey, Inc. All rights reserved.
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
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Kinvey;

namespace TestFramework
{
	[TestFixture]
	public class UserUnitTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[TearDown]
		public void Tear()
		{
		}

		[Ignore("Placeholder - No unit test yet")]
		[Test]
		public async Task TestUserBasic()
		{
			// Arrange
			Mock<RestSharp.IRestClient> moqRC = new Mock<RestSharp.IRestClient>();
			RestSharp.IRestResponse resp = new RestSharp.RestResponse();
			resp.Content = "MOCK RESPONSE";
			moqRC.Setup(m => m.ExecuteAsync(It.IsAny<RestSharp.IRestRequest>())).ReturnsAsync(resp);

			// Act

			// Assert
		}

		[Test]
		public async Task TestMICLoginAutomatedAuthFlowBad()
		{
			// Arrange
			Mock<RestSharp.IRestClient> moqRestClient = new Mock<RestSharp.IRestClient>();
			RestSharp.IRestResponse moqResponse = new RestSharp.RestResponse();

			JObject moqResponseContent = new JObject();
			moqResponseContent.Add("error", "MOCK RESPONSE ERROR");
			moqResponseContent.Add("description", "Mock Gaetway Timeout error");
			moqResponseContent.Add("debug", "Mock debug");
			moqResponse.Content = moqResponseContent.ToString();

			moqResponse.StatusCode = System.Net.HttpStatusCode.GatewayTimeout; // Status Code - 504

			moqRestClient.Setup(m => m.ExecuteAsync(It.IsAny<RestSharp.IRestRequest>())).ReturnsAsync(moqResponse);

			Client.Builder cb = new Client.Builder(TestSetup.app_key, TestSetup.app_secret)
				.setFilePath(TestSetup.db_dir)
				.setOfflinePlatform(new SQLite.Net.Platform.Generic.SQLitePlatformGeneric())
				.SetRestClient(moqRestClient.Object);

			Client c = cb.Build();
			c.MICApiVersion = "v2";

			string username = "testuser";
			string password = "testpass";
			string redirectURI = "kinveyAuthDemo://";

			// Act
			// Assert
			Exception er = Assert.CatchAsync(async delegate ()
			{
				await User.LoginWithAuthorizationCodeAPIAsync(username, password, redirectURI, c);
			});

			Assert.NotNull(er);
			KinveyException ke = er as KinveyException;
			Assert.AreEqual(EnumErrorCategory.ERROR_BACKEND, ke.ErrorCategory);
			Assert.AreEqual(EnumErrorCode.ERROR_JSON_RESPONSE, ke.ErrorCode);
			Assert.AreEqual(504, ke.StatusCode); // HttpStatusCode.GatewayTimeout
		}

        [Test]
        public async Task TestMICValidateAuthServiceID()
        {
            // Arrange
            Client.Builder builder = new Client.Builder(TestSetup.app_key, TestSetup.app_secret);
            Client client = builder.Build();
            string appKey = ((KinveyClientRequestInitializer)client.RequestInitializer).AppKey;
            string micID = "12345";
            string expectedClientId = TestSetup.app_key + "." + micID;

            // Act

            // Test AuthServiceID after setting a clientId
            var requestWithClientID = User.GetMICToken(client, "fake_code", appKey + Constants.CHAR_PERIOD + micID);
            string clientId = ((KinveyClientRequestInitializer)client.RequestInitializer).AuthServiceID;

            // Test to verify that initializing a request other than `/oauth/token` will
            // reset the AuthServiceID back to the default, which is AppKey.
            var req = User.BuildMICTempURLRequest(client, null);
            string shouldBeDefaultClientId = ((KinveyClientRequestInitializer)client.RequestInitializer).AuthServiceID;

            // Assert
            Assert.True(clientId == expectedClientId);
            Assert.True(shouldBeDefaultClientId == appKey);
        }
	}
}

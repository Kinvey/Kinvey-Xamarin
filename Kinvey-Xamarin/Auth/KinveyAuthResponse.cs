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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using Newtonsoft.Json.Linq;


namespace KinveyXamarin
{
	/// <summary>
	/// This class represents the response of a Kinvey Auth Request.
	/// </summary>
	[JsonObject]
	public class KinveyAuthResponse : JObject
    {

		/// <summary>
		/// Initializes a new instance of the <see cref="KinveyXamarin.KinveyAuthResponse"/> class.
		/// </summary>
        public KinveyAuthResponse() { }

		/// <summary>
		/// Gets or sets the user identifier.
		/// </summary>
		/// <value>The user identifier.</value>
        [JsonProperty("_id")]
        public string UserId { get; set; }

		/// <summary>
		/// Gets or sets the user metadata.
		/// </summary>
		/// <value>The user metadata.</value>
        [JsonProperty("_kmd")]
        public KinveyUserMetadata UserMetadata { get; set; }

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>The username.</value>
		[JsonProperty("username")]
		public string username { get; set; }

		[JsonExtensionData]
		public Dictionary<string, JToken> Attributes { get; set; }

		/// <summary>
		/// Gets the auth token.
		/// </summary>
		/// <value>The auth token.</value>
        public string AuthToken
        {
            get { return (UserMetadata != null ? UserMetadata.AuthToken : null); }
        }

		/// <summary>
		/// Kinvey user metadata.
		/// </summary>
		[JsonObject(MemberSerialization.OptIn)]
		public class KinveyUserMetadata : JObject
        {
			public KinveyUserMetadata() { }

			[Preserve]
			[JsonProperty("authtoken")]
			public string AuthToken { get; set; }

			[Preserve]
            [JsonProperty("lmt")]
            public string LastModifiedTime { get; set; }

			/// <summary>
			/// Gets or sets the entity creation time.
			/// </summary>
			[Preserve]
			[JsonProperty("ect")]
			public String EntityCreationTime { get; set; }

			/// <summary>
			/// Gets or sets the email verification information for a user.
			/// </summary>
			[Preserve]
			[JsonProperty("emailVerification")]
			public KMDEmailVerification EmailVerification { get; set; }
		}
    }
}

﻿// Copyright (c) 2014, Kinvey, Inc. All rights reserved.
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
using RestSharp;


namespace KinveyXamarin
{
	/// <summary>
	/// Authenticator for kinvey style authentication.
	/// </summary>
	public class KinveyAuthenticator : IAuthenticator
    {
		/// <summary>
		/// The auth header format.
		/// </summary>
        private static readonly string AuthHeaderFormat = "Kinvey {0}";
		/// <summary>
		/// The auth token.
		/// </summary>
        private readonly string authToken;

		/// <summary>
		/// Initializes a new instance of the <see cref="KinveyXamarin.KinveyAuthenticator"/> class.
		/// </summary>
		/// <param name="authToken">Auth token.</param>
		public KinveyAuthenticator(string authToken) {
			this.authToken = authToken;
		}

		/// <summary>
		/// Authenticate the specified request.
		/// </summary>
		/// <param name="client">Client.</param>
		/// <param name="request">Request.</param>
		public void Authenticate(IRestClient client, IRestRequest request) {

			if (!request.Parameters.Any(p => p.Name.Equals("Authorization", StringComparison.OrdinalIgnoreCase)))
			{
				
				var authHeader = string.Format(AuthHeaderFormat, authToken);
				request.AddParameter("Authorization", authHeader, ParameterType.HttpHeader);
			}
		}

		/// <summary>
		/// Authenticate the specified request.
		/// </summary>
		/// <param name="request">Request.</param>
		public void Authenticate(IRestRequest request) {

			if (!request.Parameters.Any(p => p.Name.Equals("Authorization", StringComparison.OrdinalIgnoreCase)))
			{

				var authHeader = string.Format(AuthHeaderFormat, authToken);
				request.AddParameter("Authorization", authHeader, ParameterType.HttpHeader);
			}
		}


    }
}

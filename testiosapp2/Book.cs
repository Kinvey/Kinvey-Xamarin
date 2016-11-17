﻿using System;
using Newtonsoft.Json;
using Kinvey;

namespace testiosapp2
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Book
	{
		[JsonProperty("_id")]
		public string BookID { get; set; }

		[JsonProperty("title")]
		public string title { get; set; }

		[JsonProperty("author")]
		public string Author { get; set; }

		[JsonProperty("date")]
		public string createdDate { get; set; }

		[JsonProperty("_kmd")]
		public KinveyMetaData metadata { get; set; }

	}
}



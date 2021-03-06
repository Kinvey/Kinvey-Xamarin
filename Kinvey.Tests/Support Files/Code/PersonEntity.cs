﻿using Newtonsoft.Json;
using SQLite;
using Kinvey;

namespace Kinvey.Tests
{
	[JsonObject(MemberSerialization.OptIn)]
	public class PersonEntity : Entity
	{
		[JsonProperty]
		public string FirstName { get; set; }

		[JsonProperty]
		public string LastName { get; set; }

		[JsonProperty]
		public AddressEntity MailAddress { get; set; }
	}
}

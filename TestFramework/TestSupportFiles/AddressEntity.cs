﻿using Newtonsoft.Json;
using KinveyXamarin;

namespace TestFramework
{
	[JsonObject(MemberSerialization.OptIn)]
	public class AddressEntity : Entity
	{
		[JsonProperty]
		public bool IsApartment { get; set; }

		[JsonProperty]
		public string Street { get; set; }
	}
}

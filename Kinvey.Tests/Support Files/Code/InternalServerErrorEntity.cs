﻿using Newtonsoft.Json;

namespace Kinvey.Tests
{
    [JsonObject(MemberSerialization.OptIn)]
    public class InternalServerErrorEntity : Entity
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}

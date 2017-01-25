using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KaktusWatch
{
    public class FbObject
    {
        public List<Data> Data { get; set; }
    }

    public class Data
    {
        public string Message { get; set; }

        [JsonProperty("created_time")]
        public DateTime CreatedTime { get; set; }
    }
}

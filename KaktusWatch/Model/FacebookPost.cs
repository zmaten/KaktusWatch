using System;
using Newtonsoft.Json;

namespace KaktusWatch.Model
{
    public class FacebookPost
    {
        public string Message { get; set; }

        [JsonProperty("created_time")]
        public DateTime CreatedTime { get; set; }
    }
}
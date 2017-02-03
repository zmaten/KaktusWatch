using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using KaktusWatch.Model;
using Newtonsoft.Json;

namespace KaktusWatch.Core
{
    public static class Worker
    {
        public static async Task<FacebookPost> GetDataAsync(string uri)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(uri);
            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookFeed>(data).Data.FirstOrDefault();
        }

        public static bool IsPromotion(FacebookPost post, int triggerInterval)
            => post.CreatedTime >= DateTimeOffset.UtcNow - new TimeSpan(0, triggerInterval, 0) &&
               post.Message.Contains("www.mujkaktus.cz/chces-pridat");
    }
}

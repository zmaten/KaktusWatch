using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KaktusWatch
{
    public static class Worker
    {
        public static async Task<FacebookFeed> GetDataAsync(string uri)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(uri);
            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookFeed>(data);
        }
           
        public static FacebookPost GetPromotion(FacebookFeed posts, int triggerInterval) 
            => posts?.Data?.FirstOrDefault(p => IsPromotion(p,triggerInterval));

        static bool IsPromotion(FacebookPost post, int triggerInterval)
            => post.CreatedTime >= DateTimeOffset.UtcNow - new TimeSpan(0, triggerInterval,0) &&
               (post.Message.Contains("www.mujkaktus.cz/chces-pridat") ||
                post.Message.Contains("kreditnavic"));
        
        public static void SendEmails(IEnumerable<string> recipients, string promotionMessage)
        {
            foreach (var recipient in recipients)
            {
                var mail = new MailMessage("kaktus.watch@gmail.com", recipient);
                var client = new SmtpClient
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Timeout = 10000,
                    Credentials = new System.Net.NetworkCredential("kaktus.watch@gmail.com", "Heslo323")
                };

                mail.Subject = String.Concat("Kaktus promotion", GetSubject(GetPromotionTimeFrame(promotionMessage)));
                mail.Body = promotionMessage;
                client.Send(mail);
            }
        }

        static string GetSubject(IEnumerable<int> timeFrame)
            => timeFrame.Count() == 2
                ? $" - {timeFrame.First()}:00 to {timeFrame.Last()}:00"
                : String.Empty;

        public static IEnumerable<int> GetPromotionTimeFrame(string message)
        {
            foreach (string word in message.Split(' '))
            {
                int hour;
                if (int.TryParse(word.Replace(".",""), out hour))
                    yield return hour;
            }
        }

    }
}

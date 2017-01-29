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

        public static async Task<FbObject> GetDataAsync(string uri)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(uri);
            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FbObject>(data);
        }
           

        public static Data GetPromotion(FbObject posts) 
            => posts?.Data?.FirstOrDefault(IsPromotion);

        static bool IsPromotion(Data post)
            => post.CreatedTime.Date == DateTime.UtcNow.Date &&
               post.CreatedTime.TimeOfDay.Hours <= DateTimeOffset.UtcNow.Hour - TriggerInterval &&
               (post.Message.Contains("www.mujkaktus.cz/chces-pridat") ||
                post.Message.Contains("kreditnavic"));

        static int TriggerInterval
            => 1;// TODO: get interval

        public static void SendEmails(IEnumerable<string> recipients, string promotionMessage)
        {
            foreach (var recipient in recipients)
            {
                var mail = new MailMessage("kaktussender@gmail.com", recipient);
                var client = new SmtpClient
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Timeout = 10000,
                    Credentials = new System.Net.NetworkCredential("kaktussender@gmail.com", "Buddy Kaktus")
                };

                mail.Subject = "Kaktus watch";
                mail.Body = GetBody(GetPromotionTimeFrame(promotionMessage));
                client.Send(mail);
            }
        }

        static string GetBody(IEnumerable<int> timeFrame)
            => timeFrame.Count() == 2
                ? $"Double credit promotion available from {timeFrame.First()} to {timeFrame.Last()}"
                : "Promotion timeframe not available.";

        public static IEnumerable<int> GetPromotionTimeFrame(string message)
        {
            foreach (string word in message.Split(' '))
            {
                int hour;
                if (int.TryParse(word, out hour))
                    yield return hour;
            }
        }

    }
}

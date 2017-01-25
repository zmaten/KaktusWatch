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

        public static async Task<FbObject> GetData(string uri)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(uri);
            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FbObject>(data);
        }

        public static bool AmIWinningLottery(FbObject posts)
        {
            if (!(posts?.Data?.Any() ?? false))
                return false;
            return posts.Data.Any(IsWinningTicket);
        }

        static bool IsWinningTicket(Data post)
            => post.CreatedTime.Date == DateTime.UtcNow.Date &&
               (post.Message.Contains("www.mujkaktus.cz/chces-pridat") ||
                post.Message.Contains("kreditnavic"));

        public static void SendEmails(IEnumerable<string> recipients)
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
                mail.Body = "";
                client.Send(mail);
            }
        }

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

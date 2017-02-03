using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace KaktusWatch.Core
{
    public class Mailing
    {
        public static SmtpClient GetClient()
            => new SmtpClient
            {
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = "smtp.gmail.com",
                EnableSsl = true,
                Timeout = 10000,
                Credentials = new NetworkCredential("kaktus.watch@gmail.com", "Heslo323")
            };

        public static void SendEmails(IEnumerable<string> recipients, string promotionMessage)
        {
            Parallel.ForEach(recipients, recipient =>
            {
                var mail = new MailMessage("kaktus.watch@gmail.com", recipient);
                var client = GetClient();

                mail.Subject = String.Concat("Kaktus promotion", GetSubject(GetPromotionTimeFrame(promotionMessage)));
                mail.Body = promotionMessage;
                client.Send(mail);
            });
        }

        static string GetSubject(IEnumerable<int> timeFrame)
            => timeFrame.Count() == 2
                ? $" - {timeFrame.First()}:00 to {timeFrame.Last()}:00"
                : "";

        public static IEnumerable<int> GetPromotionTimeFrame(string message)
        {
            foreach (string word in message.Split(' '))
            {
                int hour;
                if (Int32.TryParse(word.Replace(".", ""), out hour))
                    yield return hour;
            }
        }
    }
}

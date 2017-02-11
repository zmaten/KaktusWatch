using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace KaktusWatch.Core
{
    public class Mailing
    {
        public static SendGridAPIClient GetClient()
            => new SendGridAPIClient("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");

        public static async Task SendEmails(IEnumerable<string> recipients, string promotionMessage)
        {
            var sg = GetClient();
            var from = new Email("kaktussender@gmail.com");
            var subject = string.Concat("Kaktus promotion", GetSubject(GetPromotionTimeFrame(promotionMessage)));
            var content = new Content("text/plain", promotionMessage);
            
            foreach (var recipient in recipients)
            {
                var to = new Email(recipient);
                var mail = new Mail(from, subject, to, content);
                await sg.client.mail.send.post(requestBody: mail.Get());
            }
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
                if (int.TryParse(word.Replace(".", ""), out hour))
                    yield return hour;
            }
        }
    }
}

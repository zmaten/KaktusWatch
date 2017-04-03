using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace KaktusWatch.Core
{
    public static class Mailing
    {
        static SendGridAPIClient GetClient()
            => new SendGridAPIClient("SG"+".QB9FWm07SWm9V-Ydd1To5g.kvnnjYKdcIKT18ItzZnDgzB3hj4CYXfsRuw7X_crMK0");

        public static async Task SendEmails(IEnumerable<string> recipients, string promotionMessage)
        {
            var sg = GetClient();
            var from = new Email("kaktussender@gmail.com");
            var subject = string.Concat("Kaktus promotion", GetSubject(GetPromotionTimeFrame(promotionMessage).ToList()));
            var content = new Content("text/plain", promotionMessage);

            foreach (var recipient in recipients)
            {
                var to = new Email(recipient);
                var mail = new Mail(from, subject, to, content);
                await sg.client.mail.send.post(requestBody: mail.Get());
            }
        }

        static string GetSubject(IReadOnlyCollection<string> timeFrame)
            => timeFrame.Count == 2
                ? $" - {timeFrame.First()} to {timeFrame.Last()}"
                : "";

        public static IEnumerable<string> GetPromotionTimeFrame(string message)
        {
            bool IsHour(string word)
                => int.TryParse(word.Replace(".", "").Replace(":", ""), out int hour);
            
            return message
                .Split(' ')
                .Where(IsHour);
        }
    }
}

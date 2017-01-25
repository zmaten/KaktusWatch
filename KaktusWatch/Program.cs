using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using Microsoft.Azure.WebJobs;

namespace KaktusWatch
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage

        const string kaktusFBUrl = "https://graph.facebook.com/Kaktus/posts?access_token=1672094689755085|671e0538eaaffd57d780c950b713584c";
        const string fileLocation = "LastSentDate.kaktus";

        static IEnumerable<string> emailRecipients
            => ((NameValueCollection)ConfigurationManager.GetSection("recipientsSection")).GetValues("address");

        static void Main()
        {
            var config = new JobHostConfiguration();
            if (config.IsDevelopment)
                config.UseDevelopmentSettings();

            //var host = new JobHost();
            // The following code ensures that the WebJob will be running continuously
            //host.RunAndBlock();

            string lastSentFile;
            DateTime lastSent;
            try
            {
                lastSentFile = File.ReadAllText(fileLocation);
            }
            catch
            {
                lastSentFile = string.Empty;
            }

            DateTime.TryParse(lastSentFile, CultureInfo.InvariantCulture, DateTimeStyles.None, out lastSent);
            if (lastSent.Date == DateTime.UtcNow.Date)
                return;

            var posts = Worker.GetData(kaktusFBUrl).Result;

            if (Worker.AmIWinningLottery(posts))
            {
                Worker.SendEmails(emailRecipients);
                File.WriteAllText(fileLocation, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            }
        }

    }
}

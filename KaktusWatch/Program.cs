using System;
using System.Collections.Generic;
using System.Configuration;
using KaktusWatch.Core;
using Microsoft.Azure.WebJobs;

namespace KaktusWatch
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage

        const string kaktusFBUrl
            = "https://graph.facebook.com/Kaktus/posts?access_token=1672094689755085|671e0538eaaffd57d780c950b713584c";

        static int TriggerInterval
            => Convert.ToInt32(ConfigurationManager.AppSettings.GetValues("triggerInterval")?[0]);

        static IEnumerable<string> EmailRecipients
            => ConfigurationManager.AppSettings.GetValues("recipient");

        static void Main()
        {
            var config = new JobHostConfiguration();
            if (config.IsDevelopment)
                config.UseDevelopmentSettings();

            //var host = new JobHost();
            // The following code ensures that the WebJob will be running continuously
            //host.RunAndBlock();
            Mailing.GetClient().Send("from@from.cz", "kain@technet.ms", "Test", "telo mailu");
            var post = Worker.GetDataAsync(kaktusFBUrl).Result;
            if (Worker.IsPromotion(post, TriggerInterval))
                Mailing.SendEmails(EmailRecipients, post.Message);
        }

    }
}

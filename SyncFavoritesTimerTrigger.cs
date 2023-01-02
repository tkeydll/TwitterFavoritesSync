using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TwitterFavoritsSync
{
    public class SyncFavoritesTimerTrigger
    {
        // Source Account
        private string sourceApiKey = Environment.GetEnvironmentVariable("SOURCE_API_KEY");
        private string sourceApiSecret = Environment.GetEnvironmentVariable("SOURCE_API_SECRET");
        private string sourceAccessToken = Environment.GetEnvironmentVariable("SOURCE_ACCESS_TOKEN");
        private string sourceAccessSecret = Environment.GetEnvironmentVariable("SOURCE_ACCESS_SECRET");

        // Target Account
        private string targetApiKey = Environment.GetEnvironmentVariable("TARGET_API_KEY");
        private string targetApiSecret = Environment.GetEnvironmentVariable("TARGET_API_SECRET");
        private string targetAccessToken = Environment.GetEnvironmentVariable("TARGET_ACCESS_TOKEN");
        private string targetAccessSecret = Environment.GetEnvironmentVariable("TARGET_ACCESS_SECRET");


        [FunctionName("SyncFavoritesTimerTrigger")]
        public async Task Run([TimerTrigger("%ScheduleAppSetting%")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");


            // Get target follows.
            var accountList = TargetAccounts.GetList();
            log.LogInformation($"{accountList.Count}");

            // Get source likes.
            var sourceClient = new TwitterClient(sourceApiKey, sourceApiSecret, sourceAccessToken, sourceAccessSecret);
            var favorites = await sourceClient.GetFavoritesAsync();


            // Get target follows tweet.
            var targetTweet = favorites.Where(x => accountList.Contains(x.User.ScreenName));

            if (targetTweet.Count() == 0)
            {
                return;
            }


            // Target user
            var targetClient = new TwitterClient(targetApiKey, targetApiSecret, targetAccessToken, targetAccessSecret);

            foreach (var t in targetTweet)
            {
                await targetClient.AddFavoritsAsync(t.Id);
                log.LogInformation($"Add favorits in target account: {t.User.ScreenName}");

                await sourceClient.RemoveFavoritsAsync(t.Id);
                log.LogInformation($"Remove favorits in source account: {t.User.ScreenName}");
            }

            log.LogInformation($"Execute finished at: {DateTime.Now}");

        }
    }
}

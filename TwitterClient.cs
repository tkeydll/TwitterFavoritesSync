using CoreTweet;
using CoreTweet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterFavoritsSync
{
    public class TwitterClient
    {
        private Tokens _tokens;

        public TwitterClient(string apiKey, string apiSecret, string accessToken, string accessSecret)
        {
            _tokens = CoreTweet.Tokens.Create(apiKey, apiSecret, accessToken, accessSecret);
        }
       
        public async Task<ListedResponse<Status>> GetFavoritesAsync()
        {
            var favorites = await _tokens.Favorites.ListAsync();
            return favorites;
        }

        public async Task AddFavoritsAsync(long statusId)
        {
            await _tokens.Favorites.CreateAsync(statusId);
        }

        public async Task RemoveFavoritsAsync(long statusId)
        {
            await _tokens.Favorites.DestroyAsync(statusId);
        }
    }
}

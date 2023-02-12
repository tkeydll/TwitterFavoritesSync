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

        public async Task<StatusResponse> AddFavoritsAsync(long statusId)
        {
            return await _tokens.Favorites.CreateAsync(statusId);
        }

        public async Task<StatusResponse> RemoveFavoritsAsync(long statusId)
        {
            return await _tokens.Favorites.DestroyAsync(statusId);
        }


        public async Task<List<User>> GetAllFollowsAsync()
        {
            int cnt = 200;
            var friends = await _tokens.Friends.ListAsync(count => cnt);
            var friendsList = friends.ToList<User>();

            while (friends.Count >= cnt)
            {
                friends = await _tokens.Friends.ListAsync(cursor => friends.NextCursor, count => cnt);
                friendsList.AddRange(friends.ToList<User>());
            }

            return friendsList;
        }

    }
}


namespace TwitterFavoritsSync
{
    public class FollowUser
    {
        public long? Id;
        public string ScreenName;

        public FollowUser(long? id, string screenName)
        {
            Id = id;
            ScreenName = screenName;
        }
    }
}
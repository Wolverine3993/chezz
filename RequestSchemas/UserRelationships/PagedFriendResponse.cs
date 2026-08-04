namespace Chezz.RequestSchemas.UserRelationships
{
    public class PagedFriendResponse
    {
        public IEnumerable<FriendResponse> Friends { get; set; }
        public int Page { get; set; }
        public int FriendCount { get; set; }
    }
}

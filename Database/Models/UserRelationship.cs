namespace Chezz.Database.Models
{
    public class UserRelationship
    {
        public required string Id { get; set; }
        public required ChezzUser User1 { get; set; }
        public required string User1Id { get; set; }

        public required ChezzUser User2 { get; set; }
        public required string User2Id { get; set; }
    }
}

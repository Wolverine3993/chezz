using System.ComponentModel.DataAnnotations;

namespace Chezz.Database.Models
{
    public enum NotificationType
    {
        FriendRequest,
        MatchInvitation,
    }
    public class Notification
    {
        [Key]
        public required string Id { get; set; }
        public string UserId { get; set; }
        public NotificationType NotificationType { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CallbackId { get; set; }
    }
}

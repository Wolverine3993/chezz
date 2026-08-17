using System.ComponentModel.DataAnnotations;

namespace Chezz.Database.Models
{
    public class FriendRequest
    {
        [Key]
        public required string Id { get; set; }

        public string UserFromId { get; set; }
        public ChezzUser UserFrom { get; set; }

        public string UserToId { get; set; }
        public ChezzUser UserTo { get; set; }
    }
}

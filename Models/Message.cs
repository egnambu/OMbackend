using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OMbackend.Models
{
    public class Message
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int ShopID { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Message content cannot exceed 1000 characters")]
        public string Content { get; set; } = string.Empty;

        public bool IsUser { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? ConversationID { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ConversationID))]
        public virtual Conversation? Conversation { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(UserID))]
        public virtual User? User { get; set; }

        [ForeignKey(nameof(ShopID))]
        public virtual Shop? Shop { get; set; }
    }
}
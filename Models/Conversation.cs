using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OMbackend.Models
{
    public class Conversation
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int ShopID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;

        [Required] 
        public bool IsAdminSeen { get; set; } = false;
        
        [Required]
        public bool IsSeen { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(ShopID))]
        public virtual Shop? Shop { get; set; }

        [ForeignKey(nameof(UserID))]
        public virtual User? User { get; set; }

    }
}
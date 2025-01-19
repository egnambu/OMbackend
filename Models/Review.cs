using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OMbackend.Models
{
    public class Review
    {
        [Key]
        public int ID { get; set; }

        public int? ParentReviewID { get; set; }

        [Required]
        public int ShopID { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Username { get; set; }

        [Required] 
        [MaxLength(2000)]
        public string? Content { get; set; }

        [MaxLength(100)]
        public string? Service { get; set; }

        [Range(0, 5)]
        public float? Stars { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(ParentReviewID))]
        public virtual Review? ParentReview { get; set; }

        public virtual ICollection<Review>? Reviews { get; set; }

        [ForeignKey(nameof(ShopID))]
        public virtual Shop? Shop { get; set; }
    }
}
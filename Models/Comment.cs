using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OMbackend.Models
{
    public class Comment
    {
        [Key]
        public int ID { get; set; }

        public int? ParentCommentID { get; set; }

        [Required]
        public int PostID { get; set; }

        [Required] 
        [MaxLength(50)] 
        public string? Username { get; set; }

        [Required] 
        [MaxLength(1000)] 
        public string? Content { get; set; }

        [Range(0, int.MaxValue)]
        public int Votes { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(ParentCommentID))]
        public virtual Comment? ParentComment { get; set; }

        public virtual ICollection<Comment>? Replies { get; set; }

        [ForeignKey(nameof(PostID))]
        public virtual Post? Post { get; set; }
    }
}
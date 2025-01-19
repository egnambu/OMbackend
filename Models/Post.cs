using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OMbackend.Models
{
    public class Post
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Heading { get; set; }

        [Required]
        [MaxLength(5000)]
        public string? Content { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Username { get; set; }

        [Range(0, int.MaxValue)]
        public int? Votes { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int? Comments { get; set; } = 0;

        [MaxLength(50)]
        public string? Badge1 { get; set; } = null;

        [MaxLength(50)]
        public string? Badge2 { get; set; } = null;

        [MaxLength(50)]
        public string? Badge3 { get; set; } = null;

        [MaxLength(50)]
        public string? Badge4 { get; set; } = null;

        [MaxLength(50)]
        public string? Badge5 { get; set; } = null;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
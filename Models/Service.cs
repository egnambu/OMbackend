using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OMbackend.Models
{
    public class Service
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(30)]
        public string? Type { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Item01 { get; set; } = null;

        [MaxLength(100)]

        public string? Item02 { get; set;} = null;

        [MaxLength(100)]
        public string? Item03 { get; set;} = null;

        [MaxLength(100)]
        public string? Item04 { get; set;} = null;

        [MaxLength(100)] 
        public string? Item05 { get; set; } = null;

        [Required]
        [Range(0, float.MaxValue)]
        public float? Price { get; set; }

        [Range(-10, 100)]
        public int? InStock { get; set; }

        [Required]
        public bool IsAvailable { get; set; } = true;
    }
}
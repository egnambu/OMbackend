using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OMbackend.Models
{
    public class Shop
    {
        [Key]
        public int ID { get; set; }

        [Required] 
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(200)] 
        public string? Heading { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }


        [Range(0, int.MaxValue)] 
        public int Reviews { get; set; } = 0;
        
        [Range(0, 100)] 
        public int? Trust { get; set; }


        [Range(0, int.MaxValue)]
        public int? IconsIndex { get; set; }

        [MaxLength(500)]
        public string? Services { get; set; }


        [Column(TypeName = "float")]
        [Range(0, float.MaxValue)]
        public float? Multiplier { get; set; }

        [Column(TypeName = "float")]
        [Range(0, float.MaxValue)]
        public float? Ratings { get; set; }

        public bool? IsAvailable { get; set; } = true;

        [Range(0, 100)]
        public int? PaymentMethods { get; set; }

        [MaxLength(7)]
        public string? Users { get; set; } = null;

        [MaxLength(8)]
        public string? Color01 { get; set; } = null;

        [MaxLength(8)]
        public string? Color02 { get; set; } = null;

        [MaxLength(8)]
        public string? Color03 { get; set; } = null;

        [MaxLength(3)]
        public int? Image { get; set; }

        [MaxLength(50)]
        public string? Contact { get; set; } = null;
    }
}
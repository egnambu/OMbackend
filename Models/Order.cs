using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OMbackend.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int ShopID { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Title { get; set; }

        [Range(0, int.MaxValue)]
        public int? Status { get; set; } = 0;

        [MaxLength(50)]
        public string? PaymentReference { get; set; }

        public bool? isPaid { get; set; } = false;

        public bool? State { get; set; } = false;

        [MaxLength(255)]
        public string? Field1 { get; set; } = null;

        [MaxLength(255)]
        public string? Field2 { get; set; } = null;

        [MaxLength(255)]
        public string? Field3 { get; set; } = null;

        [MaxLength(255)]
        public string? Field4 { get; set; } = null;

        [MaxLength(255)]
        public string? Field5 { get; set; } = null;

        [Range(0, double.MaxValue)]
        public float? Price { get; set; } = null;

        [Required]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(UserID))]
        public virtual User? User { get; set; }

        [ForeignKey(nameof(ShopID))]
        public virtual Shop? Shop { get; set; }
    }
}

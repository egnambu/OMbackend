using System.ComponentModel.DataAnnotations;

namespace OMbackend.Models
{
    public class UserDto
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Username { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        public string? Password { get; set; }

        [MaxLength(200)]
        public string? SecretQuestion { get; set; }

        [MaxLength(200)]
        public string? SecretAnswer { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace OMbackend.Models
{
    public class PostDTO
    {
        public class PostCreateDto
        {
            public string? Heading { get; set; }
            public string? Content { get; set; }
            public string? Username { get; set; }
            public int? Votes { get; set; }
            public string? Badge1 { get; set; }
            public string? Badge2 { get; set; }
            public string? Badge3 { get; set; }
            public string? Badge4 { get; set; }
            public string? Badge5 { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class PostResponseDto
        {
            public int ID { get; set; }
            public string? Heading { get; set; }
            public string? Content { get; set; }
            public string? Username { get; set; }
            public int? Votes { get; set; }
            public int? Comments { get; set; }
            public string? Badge1 { get; set; }
            public string? Badge2 { get; set; }
            public string? Badge3 { get; set; }
            public string? Badge4 { get; set; }
            public string? Badge5 { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}

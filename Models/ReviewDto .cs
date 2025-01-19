namespace OMbackend.Models
{
    public class ReviewDto

    {
        public int ID { get; set; }
        public string? Content { get; set; }
        public string? Username { get; set; }
        public string? Service { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public float? Stars { get; set; }
        public List<ReviewDto>? Reviews { get; set; }
    }

}

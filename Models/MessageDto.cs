namespace OMbackend.Models
{
    public class MessageDto
    {
        public int MessageId { get; set; }
        public string Content { get; set; }
        public bool IsUser { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

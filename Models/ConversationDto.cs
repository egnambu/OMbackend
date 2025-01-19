namespace OMbackend.Models
{
   public class ConversationDto
{
    public int ID { get; set; }
    public int ConversationId { get; set; }
    public string Name { get; set; } // Assuming you want the shop name for display purposes
    public int ShopId { get; set; }
    public int UserID { get; set; }
    public bool IsSeen { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public bool IsAdminSeen { get; set; }


    }

}

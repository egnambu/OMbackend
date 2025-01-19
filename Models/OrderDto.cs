using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OMbackend.Models
{
    public class OrderDto
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
        public string Field5 { get; set; }
        public float? Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; }
    }

}

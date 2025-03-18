using FoodiesHaven.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodiesHaven.DTOs
{
    public class FeedbackDTO
    {
        public int ContactId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UserId { get; set; } // ForeignKey from Users
        
    }
}


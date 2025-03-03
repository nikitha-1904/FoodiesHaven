using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodiesHaven.Models
{
    public class Feedback
    {
        [Key]
        public int ContactId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(200)]
        public string Subject { get; set; }

        public string Message { get; set; }

        public DateTime CreatedDate { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; } // Nullable to allow anonymous contacts
        public Users User { get; set; }
    }
}
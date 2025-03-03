using System.ComponentModel.DataAnnotations;

namespace FoodiesHaven.Models
{
    public class Users
    {
        [Key]
        public int? UserId { get; set; } // Unique identifier for the user

        [Required]
        [StringLength(50)]
        public string Name { get; set; } // User Name

        [Required]
        [StringLength(12)]
        public string Username { get; set; } // User Username

        [Required]
        [StringLength(10, MinimumLength = 10)]
        public string Mobile { get; set; } // User Mobile Number

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; } // User Email

        public ICollection<string> Address { get; set; } = new List<string>(); // User Addresses

        [StringLength(6, MinimumLength = 6)]
        public string PinCode { get; set; } // User Pincode

        [Required]
        [StringLength(50)]
        public string Password { get; set; } // User Password

        public string role { get; set; } = "user"; // User role
        public DateTime CreatedDate { get; set; }  // Date the user was created

        public ICollection<Cart> Carts { get; set; } // Collection of carts associated with the user
        public ICollection<Feedback> Feedbacks { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<Orders> Order { get; set; }  // Collection of orders associated with the user
    }
}

 

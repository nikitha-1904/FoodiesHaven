using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodiesHaven.Models
{
    public class Orders
    {
        [Key]
        [Required]
        public int OrderDetailsId { get; set; } // Order number

        [Required]
        public string OrderNo { get; set; } // Order number

        [ForeignKey("Product")]
        public int ProductId { get; set; }  // Product ID 
        public Product Product { get; set; } // Navigation property to Product

        [ForeignKey("User")]
        public int UserId { get; set; } // User ID associated with the order
        public Users User { get; set; } // Navigation property to User

        [Required]
        public int Quantity { get; set; } // Quantity of the product ordered

        [Required]
        [StringLength(50)]
        public string Status { get; set; } // Status of the order

        [ForeignKey("Payment")]
        public int PaymentId { get; set; } // Payment ID associated with the order
        public Payment Payment { get; set; } // Navigation property to Payment

        public string? CouponCode { get; set; } // Nullable to allow orders without coupons
        [ForeignKey("CouponCode")]
        public Coupons? Coupons { get; set; } // Navigation property to Coupons

        public DateTime OrderDate { get; set; } // Date the order was placed
    }
}

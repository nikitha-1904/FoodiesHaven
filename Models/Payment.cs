using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodiesHaven.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; } // Unique identifier for the payment


        [StringLength(50)]
        public string PaymentStatus { get; set; }  // Name on the payment method online or offline

        [Required]
        [StringLength(50)]
        public string PaymentMode { get; set; } // Mode of payment (e.g., card, UPI, cash)


        [StringLength(16, MinimumLength = 16)]
        public string? CardNo { get; set; }  // Card number (12 digits)

        [RegularExpression(@"\d{2}/\d{2}", ErrorMessage = "Expiry date must be in the format mm/yy")]
        public string? ExpiryDate { get; set; } // Expiry date of the card (mm/yy)

        [StringLength(14, MinimumLength = 14)]
        public string? UPIId { get; set; } // UPI ID for payment (10 digits + @ybl)

        [StringLength(50)]
        public string? PaymentReceivedBy { get; set; }  // username of the person from whom the payment received

        [ForeignKey("User")]
        public int UserId { get; set; } // User ID associated with the payment
        public Users User { get; set; }// Navigation property to User

        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalAmount { get; set; } // Final amount to be paid
        public ICollection<Orders> Order { get; set; }  // Collection of orders associated with the payment
    }
}
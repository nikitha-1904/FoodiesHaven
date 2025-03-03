using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodiesHaven.Models
{
    public class Coupons
    {
        [Key]
        [Required]
        [StringLength(50)]
        public required string CouponCode { get; set; }  // Coupon code

        [StringLength(200)]
        public string Description { get; set; } // Description of the coupon

        [Required]
        [Range(0, 100)]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Discount { get; set; } // Discount percentage

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinOrderValue { get; set; } // Minimum order value to apply the coupon

        public bool IsActive { get; set; }  // Indicates if the coupon is active

    }
}

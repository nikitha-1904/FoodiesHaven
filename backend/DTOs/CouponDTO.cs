namespace FoodiesHaven.DTOs
{
    public class CouponDTO
    {
        public string CouponCode { get; set; }
        public string Description { get; set; }
        public decimal Discount { get; set; }
        public decimal MinOrderValue { get; set; }
        public bool IsActive { get; set; }

    }

    
}

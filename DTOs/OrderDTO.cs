namespace FoodiesHaven.DTOs
{
    public class OrderDTO
    {
        public string OrderNo { get; set; } // Order number
        public int ProductId { get; set; } // Product ID
        public string ProductName { get; set; } // Product name
        public int UserId { get; set; } // User ID associated with the order
        public string Username { get; set; } // Username associated with the order
        public int Quantity { get; set; } // Quantity of the product ordered
        public string Status { get; set; } // Status of the order
        public int PaymentId { get; set; } // Payment ID associated with the order
        public string CouponCode { get; set; } // Coupon code used for the order
        public DateTime OrderDate { get; set; } // Date the order was placed

    }
}

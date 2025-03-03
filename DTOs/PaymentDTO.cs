namespace FoodiesHaven.DTOs
{
    public class PaymentDTO
    {
        public string PaymentMode { get; set; }
        public string CardNo { get; set; }
        public string ExpiryDate { get; set; }
        public string UPIId { get; set; }
        //public string UserCouponName { get; set; } // Updated property name
        public string Username { get; set; }
        public int UserId { get; set; }
        public decimal FinalAmount { get; set; }
    }

    public class PaymentResultDTO //after payment we get the response
    {
        public CartDTO CartItems { get; set; } // Change this to CartDTO
        public bool IsSuccess { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountedAmount { get; set; }
        public decimal DeliveryCharge { get; set; }
        public decimal FinalAmount { get; set; }
        public string PaymentMode { get; set; }
        public string Message { get; set; }
    }

    public class PaymentDetailsDTO //in request body we will give data 
    {
        public string CardNo { get; set; }
        public string ExpiryDate { get; set; }
        public string UPIId { get; set; }
    } 

}

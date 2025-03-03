namespace FoodiesHaven.DTOs
{
    public class CartItemDTO
    {
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => ProductPrice * Quantity;
    }

    public class CartDTO
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public List<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();
        public decimal GrandTotal => Items.Sum(item => item.TotalPrice);
    }

   
}

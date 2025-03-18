namespace FoodiesHaven.DTOs
{
    public class DashboardDTO
    {
        public int TotalCategories { get; set; }
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalOfflineAmount { get; set; }
        public decimal TotalOnlineAmount { get; set; }

    }
}

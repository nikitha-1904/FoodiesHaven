using FoodiesHaven.Data;
using FoodiesHaven.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FoodiesHaven.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly EFCoreDBContext _context;

        public DashboardController(EFCoreDBContext context)
        {
            _context = context;
        }

        // GET: api/Dashboard
        [HttpGet]
        public async Task<ActionResult<DashboardDTO>> GetDashboardData()
        {
            var totalCategories = await _context.Categories.CountAsync();
            var totalProducts = await _context.Products.CountAsync();
            var totalUsers = await _context.User.CountAsync();

            // Count unique orders by OrderNo
            var uniqueOrderCount = await _context.Order
                .Select(o => o.OrderNo)
                .Distinct()
                .CountAsync();

            var totalOfflineAmount = await _context.Payments
                .Where(p => p.PaymentStatus == "successful" && p.PaymentMode == "cash")
                .SumAsync(p => p.FinalAmount);

            var totalOnlineAmount = await _context.Payments
                .Where(p => p.PaymentStatus == "successful" && (p.PaymentMode == "card" || p.PaymentMode == "upi"))
                .SumAsync(p => p.FinalAmount);

            var dashboardData = new DashboardDTO
            {
                TotalCategories = totalCategories,
                TotalProducts = totalProducts,
                TotalUsers = totalUsers,
                TotalOrders = uniqueOrderCount,
                TotalOfflineAmount = totalOfflineAmount,
                TotalOnlineAmount = totalOnlineAmount
            };

            return Ok(dashboardData);
        }
    }
}
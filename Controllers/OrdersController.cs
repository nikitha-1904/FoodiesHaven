using FoodiesHaven.Data;
using FoodiesHaven.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodiesHaven.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly EFCoreDBContext _context;

        public OrdersController(EFCoreDBContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllOrders")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllOrders()
        {
            var orders = await _context.Order
                .Include(o => o.Product)
                .Include(o => o.User)
                .Include(o => o.Payment)
                .ToListAsync();

            var groupedOrders = orders
                .GroupBy(o => new { o.UserId, o.OrderNo })
                .Select(g => new
                {
                    UserId = g.Key.UserId,
                    OrderNo = g.Key.OrderNo,
                    Orders = g.Select(o => new
                    {
                        o.ProductId,
                        ProductName = o.Product.Name,
                        o.Quantity,
                        o.Status,
                        o.PaymentId,
                        o.OrderDate,
                        o.CouponCode // Include the coupon code
                    })
                });

            return Ok(groupedOrders);
        }

        // GET: api/Order/GetUserOrderHistory/{username}
        [HttpGet("GetUserOrderHistory/{username}")]
        [Authorize(Roles = "admin,user")]

        public async Task<ActionResult<object>> GetUserOrderHistory(string username)
        {
            var user = await _context.User
                .Where(u => u.Username == username)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var orders = await _context.Order
                .Where(o => o.UserId == user.UserId)
                .Include(o => o.Product)
                .Include(o => o.Payment)
                .ToListAsync();

            var groupedOrders = orders
                .GroupBy(o => o.OrderNo)
                .Select(g => new
                {
                    OrderNo = g.Key,
                    Orders = g.Select(o => new
                    {
                        o.ProductId,
                        ProductName = o.Product.Name,
                        o.Quantity,
                        o.Status,
                        o.PaymentId,
                        o.OrderDate
                    })
                });

            return Ok(new
            {
                UserId = user.UserId,
                Username = user.Username,
                Orders = groupedOrders
            });
        }

        // GET: api/Order/GetUserRecentOrder/{username}
        [HttpGet("GetUserRecentOrder/{username}")]
        [Authorize(Roles = "admin,user")]

        public async Task<ActionResult<object>> GetUserRecentOrder(string username)
        {
            var user = await _context.User
                .Where(u => u.Username == username)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var recentOrder = await _context.Order
                .Where(o => o.UserId == user.UserId)
                .Include(o => o.Product)
                .Include(o => o.Payment)
                .OrderByDescending(o => o.OrderDate)
                .FirstOrDefaultAsync();

            if (recentOrder == null)
            {
                return NotFound("No orders found for this user.");
            }

            var orders = await _context.Order
                .Where(o => o.OrderNo == recentOrder.OrderNo)
                .Include(o => o.Product)
                .Include(o => o.Payment)
                .ToListAsync();

            var groupedOrders = orders
                .GroupBy(o => o.OrderNo)
                .Select(g => new
                {
                    OrderNo = g.Key,
                    Orders = g.Select(o => new
                    {
                        o.ProductId,
                        ProductName = o.Product.Name,
                        o.Quantity,
                        o.Status,
                        o.PaymentId,
                        o.OrderDate
                    })
                });

            return Ok(new
            {
                UserId = user.UserId,
                Username = user.Username,
                Orders = groupedOrders
            });
        }
    }
}
       
////using FoodiesHaven.Data;
////using FoodiesHaven.DTOs;
////using FoodiesHaven.Models;
////using Microsoft.AspNetCore.Http;
////using Microsoft.AspNetCore.Mvc;
////using Microsoft.EntityFrameworkCore;
////using System;
////using System.Linq;
////using System.Threading.Tasks;

////namespace FoodiesHaven.Controllers
////{
////    [Route("api/[controller]")]
////    [ApiController]
////    public class PaymentController : ControllerBase
////    {
////        private readonly EFCoreDBContext _context;

////        public PaymentController(EFCoreDBContext context)
////        {
////            _context = context;
////        }

////        [HttpPost("ProcessPayment")]
////        public async Task<ActionResult<PaymentResultDTO>> ProcessPayment(int paymentId, PaymentDTO paymentDetails)
////        {
////            var payment = await _context.Payments.FindAsync(paymentId);
////            if (payment == null)
////            {
////                return NotFound("Payment record not found.");
////            }

////            if (paymentDetails.PaymentMode == "card")
////            {
////                if (string.IsNullOrEmpty(paymentDetails.CardNo) || string.IsNullOrEmpty(paymentDetails.ExpiryDate))
////                {
////                    return BadRequest("Please enter card number and expiry date.");
////                }

////                payment.CardNo = paymentDetails.CardNo;
////                payment.ExpiryDate = paymentDetails.ExpiryDate;
////                payment.PaymentMode = "card";
////                payment.UPIId = null;
////            }
////            else if (paymentDetails.PaymentMode == "upi")
////            {
////                if (string.IsNullOrEmpty(paymentDetails.UPIId))
////                {
////                    return BadRequest("Please enter UPI ID.");
////                }

////                payment.UPIId = paymentDetails.UPIId;
////                payment.PaymentMode = "upi";
////                payment.CardNo = null;
////                payment.ExpiryDate = null;
////            }
////            else if (paymentDetails.PaymentMode == "cash")
////            {
////                payment.PaymentMode = "cash";
////                payment.CardNo = null;
////                payment.ExpiryDate = null;
////                payment.UPIId = null;
////            }
////            else
////            {
////                return BadRequest("Invalid payment mode.");
////            }

////            payment.PaymentMode = "successful";
////            _context.Entry(payment).State = EntityState.Modified;
////            await _context.SaveChangesAsync();



////            var orderNo = "ORDER" + new Random().Next(100000, 999999).ToString();
////            var cartItems = await _context.Carts
////                .Where(c => c.UserId == payment.UserId)
////                .ToListAsync();

////            var orderDate = DateTime.Now;

////            foreach (var cartItem in cartItems)
////            {
////                var order = new Orders
////                {
////                    OrderNo = orderNo,
////                    ProductId = cartItem.ProductId.Value,
////                    UserId = cartItem.UserId,
////                    Quantity = cartItem.Quantity,
////                    Status = "Order Placed",
////                    PaymentId = payment.PaymentId,
////                    OrderDate = orderDate,
////                    //CouponCode = paymentDetails.UserCouponName // Include the coupon code
////                };

////                _context.Order.Add(order);
////                _context.Carts.Remove(cartItem);
////            }

////            await _context.SaveChangesAsync();


////            // Schedule a task to update the order status to "Order Delivered" after 1 minute
////            _ = UpdateOrderStatusAfterDelay(orderNo, TimeSpan.FromMinutes(1));

////            var paymentResult = new PaymentResultDTO
////            {
////                IsSuccess = true,
////                Status = "Payment Successful",
////                ErrorMessage = null
////            };


////            return Ok(paymentResult);
////        }

////        private async Task UpdateOrderStatusAfterDelay(string orderNo, TimeSpan delay)
////        {
////            await Task.Delay(delay);

////            var ordersToUpdate = await _context.Order
////                .Where(o => o.OrderNo == orderNo)
////                .ToListAsync();

////            foreach (var order in ordersToUpdate)
////            {
////                order.Status = "Order Delivered";
////                _context.Entry(order).State = EntityState.Modified;
////            }

////            await _context.SaveChangesAsync();
////        }
////    }
////}
//using FoodiesHaven.Data;
//using FoodiesHaven.DTOs;
//using FoodiesHaven.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace FoodiesHaven.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class PaymentController : ControllerBase
//    {
//        private readonly EFCoreDBContext _context;

//        public PaymentController(EFCoreDBContext context)
//        {
//            _context = context;
//        }

//        // GET: api/Payment
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<PaymentDTO>>> GetPayments()
//        {
//            var payments = await _context.Payments
//                .Select(p => new PaymentDTO
//                {
//                    PaymentMode = p.PaymentMode,
//                    CardNo = p.CardNo,
//                    ExpiryDate = p.ExpiryDate,
//                    UPIId = p.UPIId,
//                    Username = p.User.Username,
//                    UserId = p.UserId,
//                    FinalAmount = p.FinalAmount
//                }).ToListAsync();

//            return Ok(payments);
//        }

//        // GET: api/Payment/filter
//        [HttpGet("filter")]
//        public async Task<ActionResult<IEnumerable<PaymentDTO>>> GetFilteredPayments(string paymentMode, string username)
//        {
//            // Validate payment mode
//            var validPaymentModes = new List<string> { "cash", "card", "upi" };
//            if (!string.IsNullOrEmpty(paymentMode) && !validPaymentModes.Contains(paymentMode.ToLower()))
//            {
//                return BadRequest("Invalid payment mode. Valid payment modes are cash, card, upi.");
//            }

//            // Validate username
//            if (!string.IsNullOrEmpty(username))
//            {
//                var userExists = await _context.User.AnyAsync(u => u.Username == username);
//                if (!userExists)
//                {
//                    return BadRequest("Invalid username.");
//                }
//            }

//            var query = _context.Payments.AsQueryable();

//            if (!string.IsNullOrEmpty(paymentMode))
//            {
//                query = query.Where(p => p.PaymentMode == paymentMode);
//            }

//            if (!string.IsNullOrEmpty(username))
//            {
//                query = query.Where(p => p.User.Username == username);
//            }

//            var payments = await query
//                .Select(p => new PaymentDTO
//                {
//                    PaymentMode = p.PaymentMode,
//                    CardNo = p.CardNo,
//                    ExpiryDate = p.ExpiryDate,
//                    UPIId = p.UPIId,
//                    Username = p.User.Username,
//                    UserId = p.UserId,
//                    FinalAmount = p.FinalAmount
//                }).ToListAsync();

//            return Ok(payments);
//        }
//    }
//}
using Azure;
using FoodiesHaven.Data;
using FoodiesHaven.DTOs;
using FoodiesHaven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FoodiesHaven.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly EFCoreDBContext _context;

        public PaymentController(EFCoreDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<PaymentDTO>>> GetPayments()
        {
            var payments = await _context.Payments
                .Select(p => new PaymentDTO
                {
                    PaymentId = p.PaymentId, // Add this line
                    PaymentMode = p.PaymentMode,
                    CardNo = p.CardNo,
                    ExpiryDate = p.ExpiryDate,
                    UPIId = p.UPIId,
                    Username = p.User.Username,
                    UserId = p.UserId,
                    FinalAmount = p.FinalAmount,
                    PaymentStatus = p.PaymentStatus
                }).ToListAsync();

            return Ok(payments);
        }

        [HttpGet("filter")]
        [Authorize(Roles = "admin,user")]
        public async Task<ActionResult<IEnumerable<PaymentDTO>>> GetFilteredPayments(string paymentMode, string username, string status)
        {
            var validPaymentModes = new List<string> { "cash", "card", "upi" };
            if (!string.IsNullOrEmpty(paymentMode) && !validPaymentModes.Contains(paymentMode.ToLower()))
            {
                return BadRequest("Invalid payment mode. Valid payment modes are cash, card, upi.");
            }

            if (!string.IsNullOrEmpty(username))
            {
                var userExists = await _context.User.AnyAsync(u => u.Username == username);
                if (!userExists)
                {
                    return BadRequest("Invalid username.");
                }
            }

            var validStatuses = new List<string> { "successful", "pending" };
            if (!string.IsNullOrEmpty(status) && !validStatuses.Contains(status.ToLower()))
            {
                return BadRequest("Invalid status. Valid statuses are successful, pending.");
            }

            var query = _context.Payments.AsQueryable();
            if (!string.IsNullOrEmpty(paymentMode))
            {
                query = query.Where(p => p.PaymentMode == paymentMode);
            }

            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(p => p.User.Username == username);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(p => p.PaymentStatus == status);
            }

            var payments = await query
                .Select(p => new PaymentDTO
                {
                    PaymentId = p.PaymentId, // Add this line
                    PaymentMode = p.PaymentMode,
                    CardNo = p.CardNo,
                    ExpiryDate = p.ExpiryDate,
                    UPIId = p.UPIId,
                    Username = p.User.Username,
                    UserId = p.UserId,
                    FinalAmount = p.FinalAmount,
                    PaymentStatus = p.PaymentStatus
                }).ToListAsync();

            return Ok(payments);
        }
    }
}
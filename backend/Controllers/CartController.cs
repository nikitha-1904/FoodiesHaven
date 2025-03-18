
using FoodiesHaven.Data;
using FoodiesHaven.DTOs;
using FoodiesHaven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodiesHaven.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly EFCoreDBContext _context;
        public CartController(EFCoreDBContext context)
        {
            _context = context;

        }
        //GET : api/Cart{username}
        [HttpGet("{username}")]
        [Authorize(Roles = "user")]

        public async Task<ActionResult<CartDTO>> GetCart(string username)
        {
            var user = await _context.User
                .Where(u => u.Username.ToLower() == username.ToLower())
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == user.UserId && c.Product.IsActive)
                .Select(c => new CartItemDTO
                {
                    ProductName = c.Product.Name,
                    ProductImageUrl = c.Product.ImageUrl,
                    ProductPrice = c.Product.Price,
                    Quantity = c.Quantity
                }).ToListAsync();

            if (!cartItems.Any())
            {
                return Ok($"{user.Username}'s cart is empty, browse the menu.");
            }

            var cartDTO = new CartDTO
            {
                UserId = user.UserId.Value,
                Items = cartItems
            };

            return Ok(cartDTO);
        }

        [HttpPost]
        [Authorize(Roles = "user")]

        public async Task<ActionResult<CartItemDTO>> AddToCart(string productName, string username)
        {
            try
            {
                var user = await _context.User
                    .Where(u => u.Username.ToLower() == username.ToLower())
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var product = await _context.Products
                    .Where(p => p.Name == productName && p.IsActive)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    return BadRequest("Product not found or inactive.");
                }

                var existingCart = await _context.Carts
                    .Where(c => c.UserId == user.UserId && c.ProductId == product.ProductId)
                    .FirstOrDefaultAsync();

                if (existingCart != null)
                {
                    return BadRequest("Product already in cart. Use update or delete operations for quantity changes.");
                }

                var cart = new Cart
                {
                    UserId = user.UserId.Value,
                    ProductId = product.ProductId,
                    Quantity = 1
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                var cartItemDTO = new CartItemDTO
                {
                    ProductName = product.Name,
                    ProductImageUrl = product.ImageUrl,
                    ProductPrice = product.Price,
                    Quantity = 1
                };

                return Ok(cartItemDTO);
            }
            catch (DbUpdateException ex)
            {
                // Handle database update exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the database. Please try again later.");
            }
            catch (Exception ex)
            {
                // Handle all other exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        // POST: api/Cart
        ////[HttpPost]
        ////public async Task<ActionResult<CartItemDTO>> AddToCart(string productName, string username)
        ////{
        ////    var user = await _context.User
        ////        .Where(u => u.Username.ToLower() == username.ToLower())
        ////        .FirstOrDefaultAsync();

        ////    if (user == null)
        ////    {
        ////        return NotFound("User not found.");
        ////    }

        ////    var product = await _context.Products
        ////        .Where(p => p.Name == productName && p.IsActive)
        ////        .FirstOrDefaultAsync();

        ////    if (product == null)
        ////    {
        ////        return BadRequest("Product not found or inactive.");
        ////    }

        ////    var existingCart = await _context.Carts
        ////        .Where(c => c.UserId == user.UserId && c.ProductId == product.ProductId)
        ////        .FirstOrDefaultAsync();

        ////    if (existingCart != null)
        ////    {
        ////        return BadRequest("Product already in cart. Use update or delete operations for quantity changes.");
        ////    }

        ////    var cart = new Cart
        ////    {
        ////        UserId = user.UserId.Value,
        ////        ProductId = product.ProductId,
        ////        Quantity = 1
        ////    };
        ////    _context.Carts.Add(cart);
        ////    await _context.SaveChangesAsync();

        ////    var cartItemDTO = new CartItemDTO
        ////    {
        ////        ProductName = product.Name,
        ////        ProductImageUrl = product.ImageUrl,
        ////        ProductPrice = product.Price,
        ////        Quantity = 1
        ////    };

        ////    return Ok(cartItemDTO);
        ////}


        // PUT: api/Cart
        [HttpPut]
        [Authorize(Roles = "user")]

        public async Task<IActionResult> UpdateCart(string productName, string username, int quantity = 2)
        {
            if (quantity <= 1)
            {
                return BadRequest("Quantity must be greater than one as it already exists.");
            }

            var user = await _context.User
                .Where(u => u.Username.ToLower() == username.ToLower())
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var product = await _context.Products
                .Where(p => p.Name == productName && p.IsActive)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return BadRequest("Product not found or inactive.");
            }

            var cart = await _context.Carts
                .Where(c => c.UserId == user.UserId && c.ProductId == product.ProductId)
                .FirstOrDefaultAsync();

            if (cart == null)
            {
                return NotFound("Product not found in cart.");
            }

            cart.Quantity = quantity;
            _context.Entry(cart).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // DELETE: api/Cart
        [HttpDelete]
        [Authorize(Roles = "user")]

        public async Task<IActionResult> RemoveFromCart(string productName, string username, int quantity = 1)
        {
            if (quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            var user = await _context.User
                .Where(u => u.Username.ToLower() == username.ToLower())
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var product = await _context.Products
                .Where(p => p.Name == productName && p.IsActive)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return BadRequest("Product not found or inactive.");
            }

            var cart = await _context.Carts
                .Where(c => c.UserId == user.UserId && c.ProductId == product.ProductId)
                .FirstOrDefaultAsync();

            if (cart == null)
            {
                return NotFound("Product not found in cart.");
            }

            if (cart.Quantity <= quantity)
            {
                _context.Carts.Remove(cart);
            }
            else
            {
                cart.Quantity -= quantity;
                _context.Entry(cart).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("Checkout")]
        [Authorize(Roles = "user")]

        public async Task<IActionResult> Checkout(string username, string paymentMode, PaymentDetailsDTO paymentDetails, string? couponName = null)
        {
            var user = await _context.User
                .Where(u => u.Username.ToLower() == username.ToLower())
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == user.UserId.Value && c.Product.IsActive)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return BadRequest("Cart is empty.");
            }

            decimal discount = 0;
            string usedCouponName = null;
            var totalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity);

            // Fetch product types
            var productTypes = await _context.Products
                .Where(p => cartItems.Select(c => c.ProductId).Contains(p.ProductId))
                .Select(p => p.VegNonVeg)
                .ToListAsync();

            bool allVeg = productTypes.All(pt => pt == "Veg");
            bool allNonVeg = productTypes.All(pt => pt == "NonVeg");

            if (!string.IsNullOrEmpty(couponName))
            {
                var coupon = await _context.Coupon
                    .Where(c => c.CouponCode == couponName && c.IsActive)
                    .FirstOrDefaultAsync();

                if (coupon == null)
                {
                    return BadRequest("Coupon is invalid, please apply an active coupon.");
                }

                // Validate coupon conditions
                if (coupon.CouponCode == "TASTY10")
                {
                    if (totalAmount < coupon.MinOrderValue)
                    {
                        return BadRequest($"TASTY10 coupon requires a minimum order value of {coupon.MinOrderValue}.");
                    }
                    discount = coupon.Discount;
                    usedCouponName = couponName;
                }
                else if (coupon.CouponCode == "FOODLOVE")
                {
                    if (totalAmount < coupon.MinOrderValue)
                    {
                        return BadRequest($"FOODLOVE coupon requires a minimum order value of {coupon.MinOrderValue}.");
                    }
                    discount = coupon.Discount;
                    usedCouponName = couponName;
                }
                else if (coupon.CouponCode == "FREEDINE")
                {
                    if (totalAmount < coupon.MinOrderValue)
                    {
                        return BadRequest($"FREEDINE coupon requires a minimum order value of {coupon.MinOrderValue}.");
                    }
                    discount = coupon.Discount;
                    usedCouponName = couponName;
                }
                else if (coupon.CouponCode == "WEEKENDWOW")
                {
                    if (totalAmount < coupon.MinOrderValue)
                    {
                        return BadRequest($"WEEKENDWOW coupon requires a minimum order value of {coupon.MinOrderValue}.");
                    }
                    discount = coupon.Discount;
                    usedCouponName = couponName;
                }
                else if (coupon.CouponCode == "FAMILYFEAST")
                {
                    if (totalAmount < coupon.MinOrderValue)
                    {
                        return BadRequest($"FAMILYFEAST coupon requires a minimum order value of {coupon.MinOrderValue}.");
                    }
                    discount = coupon.Discount;
                    usedCouponName = couponName;
                }
                else if (coupon.CouponCode == "VEGGIELOVER")
                {
                    if (!allVeg)
                    {
                        return BadRequest("VEGGIELOVER coupon only applies to vegetarian items.");
                    }
                    if (totalAmount < coupon.MinOrderValue)
                    {
                        return BadRequest($"VEGGIELOVER coupon requires a minimum order value of {coupon.MinOrderValue}.");
                    }
                    discount = coupon.Discount;
                    usedCouponName = couponName;
                }
                else if (coupon.CouponCode == "MEATFEAST")
                {
                    if (!allNonVeg)
                    {
                        return BadRequest("MEATFEAST coupon only applies to non-vegetarian items.");
                    }
                    if (totalAmount < coupon.MinOrderValue)
                    {
                        return BadRequest($"MEATFEAST coupon requires a minimum order value of {coupon.MinOrderValue}.");
                    }
                    discount = coupon.Discount;
                    usedCouponName = couponName;
                }
                else
                {
                    return BadRequest("Coupon conditions not met.");
                }
            }

            var discountAmount = totalAmount * discount / 100;
            var discountedAmount = totalAmount - discountAmount;
            var deliveryCharge = usedCouponName == "FREEDINE" ? 0 : 30;
            var finalAmount = discountedAmount + deliveryCharge;

            // Check for existing offline payment and replace if necessary
            var existingPayment = await _context.Payments
                .Where(p => p.UserId == user.UserId.Value && p.PaymentStatus == "offline")
                .FirstOrDefaultAsync();

            if (existingPayment != null && paymentMode == "online")
            {
                _context.Payments.Remove(existingPayment);
                await _context.SaveChangesAsync();
            }

            var payment = new Payment
            {
                UserId = user.UserId.Value,
                PaymentStatus = "pending", // Set PaymentStatus to "pending" initially
                PaymentMode = paymentMode, // Set PaymentMode based on payment details
                FinalAmount = finalAmount,
                CardNo = null,
                ExpiryDate = null,
                UPIId = null,
                PaymentReceivedBy = user.Username
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Process payment
            if (paymentMode == "card")
            {
                if (string.IsNullOrEmpty(paymentDetails.CardNo) || string.IsNullOrEmpty(paymentDetails.ExpiryDate))
                {
                    return BadRequest("Please enter card number and expiry date.");
                }

                payment.CardNo = paymentDetails.CardNo;
                payment.ExpiryDate = paymentDetails.ExpiryDate;
                payment.UPIId = null; // Ensure UPIId is null
                payment.PaymentStatus = "successful"; // Update PaymentStatus to "successful"
            }
            else if (paymentMode == "upi")
            {
                if (string.IsNullOrEmpty(paymentDetails.UPIId))
                {
                    return BadRequest("Please enter UPI ID.");
                }

                payment.UPIId = paymentDetails.UPIId;
                payment.CardNo = null; // Ensure CardNo is null
                payment.ExpiryDate = null; // Ensure ExpiryDate is null
                payment.PaymentStatus = "successful"; // Update PaymentStatus to "successful"
            }
            else if (paymentMode == "cash")
            {
                payment.PaymentStatus = "successful"; // Update PaymentStatus to "successful"
            }
            else
            {
                return BadRequest("Invalid payment mode.");
            }

            _context.Entry(payment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var orderNo = "ORDER" + new Random().Next(100000, 999999).ToString();
            var orderDate = DateTime.Now;

            foreach (var cartItem in cartItems)
            {
                var order = new Orders
                {
                    OrderNo = orderNo,
                    ProductId = cartItem.ProductId.Value,
                    UserId = cartItem.UserId,
                    Quantity = cartItem.Quantity,
                    Status = "Order Placed",
                    PaymentId = payment.PaymentId,
                    OrderDate = orderDate,
                    CouponCode = usedCouponName // Include the coupon code
                };

                _context.Order.Add(order);
                _context.Carts.Remove(cartItem);
            }

            await _context.SaveChangesAsync();
            // Ensure the address is formatted as a string
            var userAddress = string.Join(", ", user.Address);

            // Prepare cart items for response
            var cartItemsDto = new CartDTO
            {
                CartId = cartItems.First().CartId,
                UserId = user.UserId.Value,
                Items = cartItems.Select(c => new CartItemDTO
                {
                    ProductName = c.Product.Name,
                    ProductImageUrl = c.Product.ImageUrl, // Assuming there's an ImageUrl property
                    ProductPrice = c.Product.Price,
                    Quantity = c.Quantity
                }).ToList()
            };

            var paymentResult = new PaymentResultDTO
            {
                CartItems = cartItemsDto,
                IsSuccess = true,
                Status = "Payment Successful",
                ErrorMessage = null,
                GrandTotal = totalAmount,
                DiscountAmount = discountAmount,
                DiscountedAmount = discountedAmount,
                DeliveryCharge = deliveryCharge,
                FinalAmount = finalAmount,
                PaymentMode = paymentMode,
                Message = $"Your order will be delivered in 20 minutes to {userAddress}"
            };

            return Ok(paymentResult);
        }
    }
    }
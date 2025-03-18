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
    public class CouponController : ControllerBase
    {
        //private readonly EFCoreDBContext _context;

        //public CouponController(EFCoreDBContext context)
        //{
        //    _context = context;
        //}
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<CouponDTO>>> GetCoupons()
        //{
        //    var coupons = await _context.Coupon
        //        .Select(c => new CouponDTO
        //        {
        //            CouponCode = c.CouponCode,
        //            Description = c.Description,
        //            Discount = c.Discount,
        //            MinOrderValue = c.MinOrderValue,
        //            IsActive = c.IsActive
        //        }).ToListAsync();

        //    return Ok(coupons);
        //}
        private readonly EFCoreDBContext _context;
        private readonly ILogger<CouponController> _logger;

        public CouponController(EFCoreDBContext context, ILogger<CouponController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "admin,user")]

        public async Task<ActionResult<IEnumerable<CouponDTO>>> GetCoupons()
        {
            _logger.LogInformation("Getting all coupons");

            var coupons = await _context.Coupon
                .Select(c => new CouponDTO
                {
                    CouponCode = c.CouponCode,
                    Description = c.Description,
                    Discount = c.Discount,
                    MinOrderValue = c.MinOrderValue,
                    IsActive = c.IsActive
                }).ToListAsync();

            return Ok(coupons);
        }

        // POST: api/Coupon
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Coupons>> CreateCoupon(CouponDTO couponDTO)
        {
            if (string.IsNullOrEmpty(couponDTO.CouponCode) || couponDTO.Discount < 0 || couponDTO.MinOrderValue <= 0)
            {
                return BadRequest("Please fill all required fields and give correct values.");
            }

            // Check if a coupon with the same code already exists
            var existingCoupon = await _context.Coupon
                .FirstOrDefaultAsync(c => c.CouponCode == couponDTO.CouponCode);

            if (existingCoupon != null)
            {
                return BadRequest("A coupon with the same code already exists.");
            }

            var coupon = new Coupons
            {
                CouponCode = couponDTO.CouponCode,
                Description = couponDTO.Description,
                Discount = couponDTO.Discount,
                MinOrderValue = couponDTO.MinOrderValue,
                IsActive = true // Set the new coupon as active
            };

            _context.Coupon.Add(coupon);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCoupons), new { CouponCode = coupon.CouponCode }, coupon);
        }
        [HttpPut("{CouponCode}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCoupon(string CouponCode, CouponDTO couponDTO)
        {
            if (CouponCode != couponDTO.CouponCode)
            {
                return BadRequest("Coupon code mismatch.");
            }

            var coupon = await _context.Coupon.FindAsync(CouponCode);
            if (coupon == null)
            {
                return NotFound();
            }

            coupon.Description = couponDTO.Description;
            coupon.Discount = couponDTO.Discount;
            coupon.MinOrderValue = couponDTO.MinOrderValue;
            coupon.IsActive = couponDTO.IsActive;

            _context.Entry(coupon).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // DELETE: api/Coupon/5
        [Authorize(Roles = "admin")]
        [HttpDelete("{CouponCode}")]
        public async Task<IActionResult> DeleteCoupon(string CouponCode)
        {
            var coupon = await _context.Coupon.FindAsync(CouponCode);
            if (coupon == null)
            {
                return NotFound();
            }
            if (!coupon.IsActive)
            {
                return BadRequest("Category is already in inactive state.");
            }

            // Mark the coupon as inactive instead of deleting it
            coupon.IsActive = false;
            _context.Entry(coupon).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}


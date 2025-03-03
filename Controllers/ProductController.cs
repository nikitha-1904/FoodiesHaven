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
    public class ProductsController : ControllerBase
    {
        private readonly EFCoreDBContext _context;

        public ProductsController(EFCoreDBContext context)
        {
            _context = context;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await _context.Products.Include(p => p.Category)
                .Where(p => p.IsActive && p.Category.IsActive)
                .Select(p => new ProductDTO
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive,
                    CreatedDate = p.CreatedDate,
                    VegNonVeg = p.VegNonVeg,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name
                }).ToListAsync();

            return Ok(products);
        }

        // GET: api/Product/{name}
        [HttpGet("{name}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(string name)
        {
            var product = await _context.Products.Include(p => p.Category)
                .Where(p => p.Name == name)
                .Select(p => new ProductDTO
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive,
                    CreatedDate = p.CreatedDate,
                    VegNonVeg = p.VegNonVeg,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name
                }).FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // POST: api/Product
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ProductDTO>> CreateProduct(ProductDTO productDTO)
        {
           
            // Ensure all required fields are entered
            if (string.IsNullOrEmpty(productDTO.Name))
            {
                return BadRequest("Please enter the product name to add the product.");
            }

            if (string.IsNullOrEmpty(productDTO.Description))
            {
                return BadRequest("Please enter the product description to add the product.");
            }

            if (string.IsNullOrEmpty(productDTO.ImageUrl))
            {
                return BadRequest("Please enter the image URL to add the product.");
            }

            if (productDTO.Price <= 0)
            {
                return BadRequest("Please enter a valid price greater than zero to add the product.");
            }

            if (string.IsNullOrEmpty(productDTO.VegNonVeg))
            {
                return BadRequest("Please specify if the product is Veg or Non-Veg to add the product.");
            }

            if (string.IsNullOrEmpty(productDTO.CategoryName))
            {
                return BadRequest("Please enter the category name to add the product.");
            }
            // Find the category by name
            var category = await _context.Categories
                .Where(c => c.Name.ToLower() == productDTO.CategoryName.ToLower() && c.IsActive)
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return BadRequest("Invalid or inactive category.");
            }

            // Check if a product with the same name or ID already exists
            var existingProductByName = await _context.Products
                .Where(p => p.Name == productDTO.Name && p.IsActive)
                .FirstOrDefaultAsync();

            var existingProductById = await _context.Products
                .Where(p => p.ProductId == productDTO.ProductId && p.IsActive)
                .FirstOrDefaultAsync();

            var existingProductByImageUrl = await _context.Products
                .Where(p => p.ImageUrl == productDTO.ImageUrl && p.IsActive)
                .FirstOrDefaultAsync();

            if (existingProductByName != null && existingProductById != null)
            {
                return BadRequest($"Both product ID {productDTO.ProductId} and name {productDTO.Name} already exist. Try to give different ones.");
            }

            if (existingProductByName != null)
            {
                return BadRequest($"Product name {productDTO.Name} already exists.");
            }

            if (existingProductById != null)
            {
                return BadRequest($"Product ID {productDTO.ProductId} already exists.");
            }

            if (existingProductByImageUrl != null)
            {
                return BadRequest("Image URL already exists. Same URL not allowed.");
            }

            // Additional validations
            if (productDTO.Price <= 0)
            {
                return BadRequest("Product price must be greater than zero.");
            }

            if (string.IsNullOrEmpty(productDTO.ImageUrl))
            {
                return BadRequest("Image URL is required.");
            }

            if (productDTO.Name.Length > 100)
            {
                return BadRequest("Product name must be 100 characters or less.");
            }

            if (productDTO.Description.Length > 500)
            {
                return BadRequest("Product description must be 500 characters or less.");
            }

            if (!Uri.IsWellFormedUriString(productDTO.ImageUrl, UriKind.Absolute))
            {
                return BadRequest("Invalid image URL format.");
            }

            if (productDTO.CreatedDate > DateTime.Now)
            {
                return BadRequest("Created date cannot be in the future.");
            }

            var product = new Product
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                ImageUrl = productDTO.ImageUrl,
                IsActive = productDTO.IsActive,
                CreatedDate = DateTime.Now,
                VegNonVeg = productDTO.VegNonVeg,
                CategoryId = category.CategoryId, // Use the category ID from the found category
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var simpleProductDTO = new ProductDTO
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                CreatedDate = product.CreatedDate,
                VegNonVeg = product.VegNonVeg,
                CategoryId = product.CategoryId,
                CategoryName = category.Name
            };

            return CreatedAtAction(nameof(GetProduct), new { name = product.Name }, simpleProductDTO);
        }

        // PUT: api/Product/{name}
        [Authorize(Roles = "admin")]
        [HttpPut("{name}")]
        public async Task<IActionResult> UpdateProduct(string name, ProductDTO productDTO)
        {
            var product = await _context.Products
                .Where(p => p.Name == name)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            // Find the category by name
            var category = await _context.Categories
                .Where(c => c.Name.ToLower() == productDTO.CategoryName.ToLower() && c.IsActive)
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return BadRequest("Invalid or inactive category.");
            }

            // Check if a product with the same name or image URL already exists (excluding the current product)
            var existingProductByName = await _context.Products
                .Where(p => p.Name == productDTO.Name && p.ProductId != product.ProductId && p.IsActive)
                .FirstOrDefaultAsync();

            var existingProductByImageUrl = await _context.Products
                .Where(p => p.ImageUrl == productDTO.ImageUrl && p.ProductId != product.ProductId && p.IsActive)
                .FirstOrDefaultAsync();

            if (existingProductByName != null)
            {
                return BadRequest($"Product name {productDTO.Name} already exists.");
            }

            if (existingProductByImageUrl != null)
            {
                return BadRequest("Image URL already exists. Same URL not allowed.");
            }

            // Additional validations
            if (productDTO.Price <= 0)
            {
                return BadRequest("Product price must be greater than zero.");
            }

            if (string.IsNullOrEmpty(productDTO.ImageUrl))
            {
                return BadRequest("Image URL is required.");
            }

            if (productDTO.Name.Length > 100)
            {
                return BadRequest("Product name must be 100 characters or less.");
            }

            if (productDTO.Description.Length > 500)
            {
                return BadRequest("Product description must be 500 characters or less.");
            }

            if (!Uri.IsWellFormedUriString(productDTO.ImageUrl, UriKind.Absolute))
            {
                return BadRequest("Invalid image URL format.");
            }

            if (productDTO.CreatedDate > DateTime.Now)
            {
                return BadRequest("Created date cannot be in the future.");
            }

            product.Name = productDTO.Name;
            product.Description = productDTO.Description;
            product.Price = productDTO.Price;
            product.ImageUrl = productDTO.ImageUrl;
            product.IsActive = productDTO.IsActive;
            product.VegNonVeg = productDTO.VegNonVeg;
            product.CategoryId = category.CategoryId; // Use the category ID from the found category

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Product/{name}
        [Authorize(Roles = "admin")]
        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteProduct(string name)
        {
            var product = await _context.Products
                .Where(p => p.Name == name)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            product.IsActive = false;
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
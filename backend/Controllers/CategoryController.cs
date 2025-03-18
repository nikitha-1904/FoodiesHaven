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
    public class CategoryController : ControllerBase
    {
        private readonly EFCoreDBContext _context;

        public CategoryController(EFCoreDBContext context)
        {
            _context = context;
        }

        // GET: api/Category
        [HttpGet]
        [Authorize(Roles = "admin,user")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .Select(c => new CategoryDTO
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    Products = c.Products.Select(p => new CategoryProductDTO
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        ImageUrl = p.ImageUrl,
                        IsActive = p.IsActive,
                        CreatedDate = p.CreatedDate,
                        VegNonVeg = p.VegNonVeg,
                    }).ToList()
                }).ToListAsync();

            return Ok(categories);
        }

        // GET: api/Category/filter
        [HttpGet("filter")]
        [Authorize(Roles = "admin,user")]
        public async Task<ActionResult<IEnumerable<ProductFilteringDTO>>> GetFilteredProducts([FromQuery] string type, [FromQuery] string categoryName)
        {
            IQueryable<Product> query = _context.Products;

            if (!string.IsNullOrEmpty(categoryName))
            {
                query = query.Where(p => p.Category.Name == categoryName);
            }

            if (type == "Veg")
            {
                query = query.Where(p => p.VegNonVeg == "Veg");
            }
            else if (type == "NonVeg")
            {
                query = query.Where(p => p.VegNonVeg == "NonVeg");
            }
            else if (type != "Both" && type != null)
            {
                return BadRequest("Enter a valid input to filter (Veg, NonVeg, Both).");
            }

            var products = await query
                .Select(p => new ProductFilteringDTO
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    VegNonVeg = p.VegNonVeg,
                    CategoryId = p.CategoryId
                })
                .ToListAsync();

            return Ok(products);
        }





        // POST: api/Category
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Category>> CreateCategory(CategoryCreateUpdateDTO categoryDTO)
        {
            // Check for null or empty values
            if (string.IsNullOrEmpty(categoryDTO.Name))
            {
                return BadRequest("Please fill all fields to add a category.");
            }

            // Check if a category with the same ID already exists
            var existingCategoryById = await _context.Categories.FindAsync(categoryDTO.CategoryId);
            var existingCategoryByName = await _context.Categories
                .Where(c => c.Name == categoryDTO.Name)
                .FirstOrDefaultAsync();

            if (existingCategoryById != null && existingCategoryByName != null)
            {
                return BadRequest($"Category ID already exists for {existingCategoryById.Name} and {categoryDTO.Name} already exists.");
            }

            if (existingCategoryById != null)
            {
                return BadRequest($"Category ID already exists for {existingCategoryById.Name}.");
            }

            if (existingCategoryByName != null)
            {
                return BadRequest("Category with the same name already exists.");
            }

            var category = new Category
            {
                Name = categoryDTO.Name,
                IsActive = categoryDTO.IsActive
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoryByName), new { name = category.Name }, category);
        }

        // GET: api/Category/{name}
        [HttpGet("{name}")]
        [Authorize(Roles = "admin,user")]
        public async Task<ActionResult<CategoryDTO>> GetCategoryByName(string name)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .Where(c => c.Name.ToLower() == name.ToLower())
                .Select(c => new CategoryDTO
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    Products = c.Products.Select(p => new CategoryProductDTO
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        ImageUrl = p.ImageUrl,
                        IsActive = p.IsActive,
                        CreatedDate = p.CreatedDate,
                        VegNonVeg = p.VegNonVeg,
                    }).ToList()
                }).FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        
        // PUT: api/Category/{name}
        [HttpPut("{name}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCategory(string name, CategoryCreateUpdateDTO categoryDTO)
        {
            var category = await _context.Categories
                .Where(c => c.Name.ToLower() == name.ToLower())
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound(new { Message = $"Category with name '{name}' not found." });
            }

            category.Name = categoryDTO.Name;
            category.IsActive = categoryDTO.IsActive;

            _context.Entry(category).State = EntityState.Modified;

            if (!category.IsActive)
            {
                var products = await _context.Products.Where(p => p.CategoryId == category.CategoryId).ToListAsync();
                foreach (var product in products)
                {
                    product.IsActive = false;
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }


        // DELETE: api/Category/{name}
        [Authorize(Roles = "admin")]
        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteCategory(string name)
        {
            var category = await _context.Categories
                .Where(c => c.Name.ToLower() == name.ToLower())
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound();
            }

            if (!category.IsActive)
            {
                return BadRequest("Category is already in inactive state.");
            }

            category.IsActive = false;
            _context.Entry(category).State = EntityState.Modified;

            var products = await _context.Products.Where(p => p.CategoryId == category.CategoryId).ToListAsync();
            foreach (var product in products)
            {
                product.IsActive = false;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
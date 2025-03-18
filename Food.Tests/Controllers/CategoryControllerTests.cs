using FoodiesHaven.Controllers;
using FoodiesHaven.Data;
using FoodiesHaven.DTOs;
using FoodiesHaven.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodiesHaven.Tests.Controllers
{
    [TestFixture]
    public class CategoryControllerTests
    {
        private EFCoreDBContext _context;
        private Mock<ILogger<CategoryController>> _mockLogger;
        private CategoryController _controller;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<EFCoreDBContext>()
                .UseInMemoryDatabase(databaseName: "FoodiesHavenTest")
                .Options;

            _context = new EFCoreDBContext(options);
            _mockLogger = new Mock<ILogger<CategoryController>>();
            _controller = new CategoryController(_context);

            ClearDatabase();
            SeedDatabase();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private void ClearDatabase()
        {
            _context.Categories.RemoveRange(_context.Categories);
            _context.Products.RemoveRange(_context.Products);
            _context.SaveChanges();
        }

        private void SeedDatabase()
        {
            var category = new Category
            {
                CategoryId = 1,
                Name = "Test Category",
                IsActive = true,
                Products = new List<Product>()
            };

            var product = new Product
            {
                ProductId = 1,
                Name = "Test Product",
                Description = "Test Description",
                Price = 100m,
                ImageUrl = "http://example.com/image.jpg",
                IsActive = true,
                CreatedDate = DateTime.Now,
                VegNonVeg = "Veg",
                Category = category
            };

            category.Products.Add(product);

            _context.Categories.Add(category);
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        [Test]
        public async Task GetCategories_ReturnsOkResult_WithListOfCategories()
        {
            // Act
            var result = await _controller.GetCategories();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<List<CategoryDTO>>());
            var categoryList = okResult.Value as List<CategoryDTO>;
            Assert.That(categoryList.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetFilteredProducts_ReturnsOkResult_WithListOfProducts()
        {
            // Act
            var result = await _controller.GetFilteredProducts("Veg", "Test Category");

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<List<ProductFilteringDTO>>());
            var productList = okResult.Value as List<ProductFilteringDTO>;
            Assert.That(productList.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetFilteredProducts_ReturnsBadRequest_WhenInvalidType()
        {
            // Act
            var result = await _controller.GetFilteredProducts("InvalidType", "Test Category");

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task CreateCategory_ReturnsCreatedAtActionResult_WithCategory()
        {
            // Arrange
            var categoryDto = new CategoryCreateUpdateDTO
            {
                CategoryId = 2,
                Name = "New Category",
                IsActive = true
            };

            // Act
            var result = await _controller.CreateCategory(categoryDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.That(createdResult.Value, Is.InstanceOf<Category>());
        }

        [Test]
        public async Task CreateCategory_ReturnsBadRequest_WhenCategoryExists()
        {
            // Arrange
            var categoryDto = new CategoryCreateUpdateDTO
            {
                CategoryId = 1,
                Name = "Test Category",
                IsActive = true
            };

            // Act
            var result = await _controller.CreateCategory(categoryDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetCategoryByName_ReturnsOkResult_WithCategory()
        {
            // Act
            var result = await _controller.GetCategoryByName("Test Category");

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<CategoryDTO>());
        }

        [Test]
        public async Task GetCategoryByName_ReturnsNotFound_WhenCategoryNotFound()
        {
            // Act
            var result = await _controller.GetCategoryByName("NonExistent Category");

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task UpdateCategory_ReturnsNoContent_WhenCategoryUpdated()
        {
            // Arrange
            var categoryDto = new CategoryCreateUpdateDTO
            {
                CategoryId = 1,
                Name = "Updated Category",
                IsActive = true
            };

            // Act
            var result = await _controller.UpdateCategory("Test Category", categoryDto);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task UpdateCategory_ReturnsNotFound_WhenCategoryNotFound()
        {
            // Arrange
            var categoryDto = new CategoryCreateUpdateDTO
            {
                CategoryId = 99,
                Name = "NonExistent Category",
                IsActive = true
            };

            // Act
            var result = await _controller.UpdateCategory("NonExistent Category", categoryDto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task DeleteCategory_ReturnsNoContent_WhenCategoryDeleted()
        {
            // Act
            var result = await _controller.DeleteCategory("Test Category");

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteCategory_ReturnsNotFound_WhenCategoryNotFound()
        {
            // Act
            var result = await _controller.DeleteCategory("NonExistent Category");

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteCategory_ReturnsBadRequest_WhenCategoryAlreadyInactive()
        {
            // Arrange
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Test Category");
            category.IsActive = false;
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteCategory("Test Category");

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}
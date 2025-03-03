namespace FoodiesHaven.DTOs
{
    public class ProductDTO //To create products
    {
        public int ProductId { get; set; } // Unique identifier for the product
        public string Name { get; set; } // Product name
        public string Description { get; set; } // Product description
        public decimal Price { get; set; }  // Product price
        public string ImageUrl { get; set; }  // URL of the product image
        public bool IsActive { get; set; }  // Indicates if the product is active
        public DateTime CreatedDate { get; set; }  // Date the product was created
        public string VegNonVeg { get; set; } // Indicates if the product is vegetarian or non-vegetarian
        public int CategoryId { get; set; } // Foreign key for Category
        public string CategoryName { get; set; } // Category name
    }

    public class CategoryProductDTO //in under each category to specify the total products
    {
            public int ProductId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public string ImageUrl { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedDate { get; set; }
            public string VegNonVeg { get; set; }
        
    }

    public class ProductFilteringDTO
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string VegNonVeg { get; set; } // "Veg" or "NonVeg"
        public int CategoryId { get; set; }
    }
}

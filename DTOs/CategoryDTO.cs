namespace FoodiesHaven.DTOs
{
    public class CategoryDTO
    {

        public int CategoryId { get; set; } // Unique identifier for the category
        public string Name { get; set; } // Category name
        public bool IsActive { get; set; } // Indicates if the category is active

        public List<CategoryProductDTO> Products { get; set; } // Add this line
    }

    public class CategoryCreateUpdateDTO
    {
        public int CategoryId { get; set; } // Unique identifier for the category
        public string Name { get; set; } // Category name
        public bool IsActive { get; set; } // Indicates if the category is active

    }
}

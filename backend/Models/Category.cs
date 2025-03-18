using System.ComponentModel.DataAnnotations;

namespace FoodiesHaven.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; } // Unique identifier for the category

        [Required]
        [StringLength(50)]
        public string Name { get; set; } // Category name

        public bool IsActive { get; set; } // Indicates if the category is active



        public ICollection<Product> Products { get; set; } // Collection of products in the category
    }
}

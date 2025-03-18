using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FoodiesHaven.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; } // Unique identifier for the product


        [Required]
        [StringLength(50)]
        public string Name { get; set; } // Product name

        [StringLength(500)] // Increased length for detailed descriptions
        public string Description { get; set; } // Product description


        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }  // Product price

        public string ImageUrl { get; set; }  // URL of the product image

        public bool IsActive { get; set; }  // Indicates if the product is active

        public DateTime CreatedDate { get; set; }  // Date the product was created

        [Required]
        [StringLength(10)]
        public string VegNonVeg { get; set; } // Indicates if the product is vegetarian or non-vegetarian
        // Category properties merged into Product

        [Required]
        public int CategoryId { get; set; } // Foreign key for Category

        [ForeignKey("CategoryId")]
        [JsonIgnore]
        public Category Category { get; set; } // Navigation property
        public ICollection<Cart> Carts { get; set; } // Collection of carts containing the product
        public ICollection<Orders> Order { get; set; } //Collection of orders containing the product

    }
}
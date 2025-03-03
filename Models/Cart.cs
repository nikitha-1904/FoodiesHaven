using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FoodiesHaven.Models;

public class Cart
{
    [Key]
    public int CartId { get; set; } // Unique identifier for the cart

    [ForeignKey("Product")]
    //Nullable Foreign Key: Allows the cart to remain in the database even if the product is deleted.
    //DeleteBehavior.SetNull: Ensures that the ProductId is set to null when the product is deleted, preserving the cart.
    // when a product is deleted, the ProductId in the Cart table will be set to null.
    // This ensures that the cart remains in the database even if the product it was associated with is deleted.
    public int? ProductId { get; set; } // Nullable Product ID associated with the cart
    public Product Product { get; set; } // Navigation property to Product

    [ForeignKey("User")]
    public int UserId { get; set; } // User ID associated with the cart
    public Users User { get; set; } // Navigation property to User

    [Required]
    public int Quantity { get; set; } // Quantity of the product in the cart

    

}
using FoodiesHaven.DTOs;
using FoodiesHaven.Models;
using Microsoft.EntityFrameworkCore;
namespace FoodiesHaven.Data
{
    public class EFCoreDBContext : DbContext
    {
        public EFCoreDBContext(DbContextOptions options) : base(options) //Constructor that initializes the database context with options.
        {
        }
        public DbSet<Users> User { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Orders> Order { get; set; }
        public DbSet<Coupons> Coupon { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User related entities
            //DeleteBehavior.Cascade: Ensures that deleting a user also deletes all related carts.
            //No Cascade on Cart Deletion: Deleting a cart does not affect the user.
            modelBuilder.Entity<Users>()
                .HasMany(u => u.Carts)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //DeleteBehavior.Cascade: Ensures that deleting a user also deletes all related orders.
            //No Cascade on Order Deletion: Deleting an order does not affect the user.
            modelBuilder.Entity<Users>()
                .HasMany(u => u.Order)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //DeleteBehavior.Cascade: Ensures that deleting a user also deletes all related payment methods.
            //No Cascade on Payment Deletion: Deleting a payment method does not affect the user.
            modelBuilder.Entity<Users>()
                .HasMany(u => u.Payments)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //DeleteBehavior.SetNull: Ensures that when a user is deleted, the UserId in the Contact table is set to null, preserving the feedback forms.
            //Nullable Foreign Key: The UserId in the Contact class is made nullable to support this behavior.
            modelBuilder.Entity<Users>()
                .HasMany(u => u.Feedbacks)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Products and related entities
            //When a product is deleted, the ProductId in the Cart table is set to null.
            //This ensures that the cart remains, but the reference to the deleted product is removed.
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Product)
                .WithMany(p => p.Carts)
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.SetNull);

            //One Product in Many Orders: A single product can be part of multiple orders.
            //DeleteBehavior.NoAction: This configuration ensures that no action is taken on the Orders table when a product is deleted.
            //This prevents the deletion of orders that reference the deleted product, maintaining the integrity of the order history.
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Order)
                .WithOne(o => o.Product)
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            // Payments and related entities
            //One Payment Method in Many Orders: A single payment method can be used for multiple orders.
            //DeleteBehavior.NoAction: This configuration ensures that no action is taken on the Orders table when a payment method is deleted.
            //This prevents the deletion of orders that reference the deleted payment method,
            modelBuilder.Entity<Payment>()
                .HasMany(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey(o => o.PaymentId)
                .OnDelete(DeleteBehavior.NoAction);

            // Orders and related entities
            //DeleteBehavior.SetNull: Ensures that when a coupon is deleted, the CouponId in the Order table is set to null, preserving the orders.
            //DeleteBehavior.SetNull: This configuration ensures that when a coupon is deleted, the CouponId in the Order table is set to null.
            //This prevents the deletion of orders that reference the deleted coupon,
            modelBuilder.Entity<Orders>()
                .HasOne(o => o.Coupons)
                .WithMany()
                .HasForeignKey(o => o.CouponCode)
                .OnDelete(DeleteBehavior.SetNull);


            // Category-Product relationship
            //Category-Product Relationship:
            //DeleteBehavior.Cascade ensures that when a category is deleted, all associated products are also deleted.
            //Deleting a product will not affect the category.
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}

// DbSet - collection of entities of a specific type that you can query from the database. Each
// DbSet corresponds to a table in the database.

//EFCoreDbContext - inherits from the DbContext , represents database context

//OnmodelCreating - This method is used to configure the model using the ModelBuilder.

//ModelBuilder - configure the shape of your entities, the relationships between them, and how they map to the database.

//Repository Pattern -  The DbContext class acts as a repository that provides a way to interact with the database. It abstracts the data access logic and provides a clean API for querying and saving data.




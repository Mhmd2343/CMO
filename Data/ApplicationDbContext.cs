using DeliveryAppSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAppSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<DeliveryRequest> DeliveryRequests { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShopOrder> ShopOrders { get; set; }
        public DbSet<ShopOrderItem> ShopOrderItems { get; set; }

        public DbSet<RideRequest> RideRequests { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Driver ↔ User one-to-one relationship
            modelBuilder.Entity<Driver>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Rating ↔ Customer
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Customer)
                .WithMany()
                .HasForeignKey(r => r.CustomerID)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<ShopOrder>()
                .HasMany(o => o.Items)
                .WithOne(i => i.ShopOrder)
                .HasForeignKey(i => i.ShopOrderId);

            // Rating ↔ Driver
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Driver)
                .WithMany()
                .HasForeignKey(r => r.DriverID)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Seed Grocery Product Categories
            modelBuilder.Entity<ProductCategory>().HasData(
                new ProductCategory { ProductCategoryId = 1, Name = "Fresh" },
                new ProductCategory { ProductCategoryId = 2, Name = "Snacks" },
                new ProductCategory { ProductCategoryId = 3, Name = "Soft Drinks" },
                new ProductCategory { ProductCategoryId = 4, Name = "Dairy" }
            );

            // ✅ Seed a Shop
            modelBuilder.Entity<Shop>().HasData(
                new Shop { ShopId = 1, Name = "QuickMart", Location = "Downtown" }
            );

            // ✅ Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    Name = "Apple",
                    Price = 1.50m,
                    Description = "Fresh red apples",
                    ShopId = 1,
                    ProductCategoryId = 1
                },
                new Product
                {
                    ProductId = 2,
                    Name = "Potato Chips",
                    Price = 2.50m,
                    Description = "Crispy and salty",
                    ShopId = 1,
                    ProductCategoryId = 2
                },
                new Product
                {
                    ProductId = 3,
                    Name = "Orange Juice",
                    Price = 3.00m,
                    Description = "Freshly squeezed juice",
                    ShopId = 1,
                    ProductCategoryId = 3
                },
                new Product
                {
                    ProductId = 4,
                    Name = "Cheddar Cheese",
                    Price = 4.00m,
                    Description = "Aged cheddar cheese block",
                    ShopId = 1,
                    ProductCategoryId = 4
                }
            );

            // Optional: Define relationship (if not already in model annotations)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.ProductCategoryId);
        }


    }
}

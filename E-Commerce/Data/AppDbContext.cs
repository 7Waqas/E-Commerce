using Microsoft.EntityFrameworkCore;
using E_Commerce.Models;

namespace E_Commerce.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Gadgets and devices", CreatedAt = DateTime.SpecifyKind(new DateTime(2025, 1, 1), DateTimeKind.Utc) },
                new Category { Id = 2, Name = "Fashion", Description = "Clothing and accessories", CreatedAt = DateTime.SpecifyKind(new DateTime(2025, 1, 1), DateTimeKind.Utc) },
                new Category { Id = 3, Name = "Home & Living", Description = "Furniture and home decor", CreatedAt = DateTime.SpecifyKind(new DateTime(2025, 1, 1), DateTimeKind.Utc) },
                new Category { Id = 4, Name = "Sports", Description = "Sports and outdoor gear", CreatedAt = DateTime.SpecifyKind(new DateTime(2025, 1, 1), DateTimeKind.Utc) },
                new Category { Id = 5, Name = "Books", Description = "Books and stationery", CreatedAt = DateTime.SpecifyKind(new DateTime(2025, 1, 1), DateTimeKind.Utc) }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Sony WH-1000XM5 Headphones",
                    Description = "Industry-leading noise canceling with 30-hour battery life.",
                    ProductUrl = "https://www.amazon.com",
                    ImagePath = null,
                    CategoryId = 1,
                    CreatedAt = DateTime.SpecifyKind(new DateTime(2025, 1, 1), DateTimeKind.Utc)
                }
            );
        }
    }
}
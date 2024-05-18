using Core.Entities.Identity;
using Core.Entities.OrderAggregate;
using Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        //private void SeedData(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<ProductType>().HasData(
        //        new ProductType { Id = 1, Name = "Electronics" },
        //        new ProductType { Id = 2, Name = "Clothing" }
        //    );

        //    modelBuilder.Entity<ProductBrand>().HasData(
        //        new ProductBrand { Id = 1, Name = "Brand A" },
        //        new ProductBrand { Id = 2, Name = "Brand B" }
        //        // Add more product brands as needed
        //    );

        //    // Seed Products
        //    modelBuilder.Entity<Product>().HasData(
        //        new Product
        //        {
        //            Id = 1,
        //            Name = "Product 1",
        //            Description = "Description for Product 1",
        //            Price = 99.99m,
        //            PictureUrl = "product1.jpg",
        //            ProductTypeId = 1, // Assuming you have product type IDs defined
        //            ProductBrandId = 1 // Assuming you have product brand IDs defined
        //        },
        //        new Product
        //        {
        //            Id = 2,
        //            Name = "Product 2",
        //            Description = "Description for Product 2",
        //            Price = 149.99m,
        //            PictureUrl = "product2.jpg",
        //            ProductTypeId = 2,
        //            ProductBrandId = 2
        //        }
        //        // Add more products as needed
        //    );
        //}
    }
}

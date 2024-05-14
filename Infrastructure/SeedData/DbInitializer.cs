//using Core.Models;
//using Infrastructure.Data;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Infrastructure.SeedData
//{
//    public class DbInitializer
//    {
//        public static async Task SeedAsync(ApplicationDbContext context, ILoggerFactory loggerFactory)
//        {
//            try
//            {
//                if (!context.Products.Any())
//                {
//                    // Add fake products
//                    var products = new List<Product>
//                    {
//                        new Product { Name = "Product 1", Description = "Description for Product 1", Price = 99.99m, PictureUrl = "product1.jpg" },
//                        new Product { Name = "Product 2", Description = "Description for Product 2", Price = 149.99m, PictureUrl = "product2.jpg" },
//                        new Product { Name = "Product 3", Description = "Description for Product 3", Price = 199.99m, PictureUrl = "product3.jpg" }
//                        // Add more fake products as needed
//                    };

//                    context.Products.AddRange(products);
//                    await context.SaveChangesAsync();
//                }

//                if (!context.ProductTypes.Any())
//                {
//                    // Add fake product types
//                    var productTypes = new List<ProductType>
//                    {
//                        new ProductType { Name = "Electronics" },
//                        new ProductType { Name = "Clothing" },
//                        new ProductType { Name = "Books" }
//                        // Add more fake product types as needed
//                    };

//                    context.ProductTypes.AddRange(productTypes);
//                    await context.SaveChangesAsync();
//                }

//                if (!context.ProductBrands.Any())
//                {
//                    // Add fake product brands
//                    var productBrands = new List<ProductBrand>
//                    {
//                        new ProductBrand { Name = "Brand A" },
//                        new ProductBrand { Name = "Brand B" },
//                        new ProductBrand { Name = "Brand C" }
//                        // Add more fake product brands as needed
//                    };

//                    context.ProductBrands.AddRange(productBrands);
//                    await context.SaveChangesAsync();
//                }
//            }
//            catch (Exception ex)
//            {
//                var logger = loggerFactory.CreateLogger(typeof(DbInitializer));
//                logger.LogError(ex.Message);
//            }
//        }
//    }
//}

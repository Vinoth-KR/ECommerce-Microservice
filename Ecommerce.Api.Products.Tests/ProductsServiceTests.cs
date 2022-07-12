using NUnit.Framework;
using ECommerce.Api.Products.Db;
using Microsoft.EntityFrameworkCore;
using System;
using ECommerce.Api.Products.Profiles;
using AutoMapper;
using ECommerce.Api.Products.Providers;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Api.Products.Tests
{
    public class ProductsServiceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetProductsReturnsAllProducts()
        {
            var options = new DbContextOptionsBuilder<ProductsDbContext>()
                .UseInMemoryDatabase(nameof(GetProductsReturnsAllProducts)).Options;
            var dbContext = new ProductsDbContext(options);

            CreateProducts(dbContext);

            var productsProfile = new ProductProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(productsProfile));
            var mapper = new Mapper(configuration);

            var productsProvider = new ProductsProvider(dbContext, null, mapper);

            var product = await productsProvider.GetProductsAsync();

            Assert.True(product.IsSuccess);
            Assert.True(product.Products.Any());
            Assert.Null(product.ErrorMessage);
        }

        [Test]
        public async Task GetProductsReturnsProductUsingValidID()
        {
            var options = new DbContextOptionsBuilder<ProductsDbContext>()
                .UseInMemoryDatabase(nameof(GetProductsReturnsAllProducts)).Options;
            var dbContext = new ProductsDbContext(options);

            CreateProducts(dbContext);

            var productsProfile = new ProductProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(productsProfile));
            var mapper = new Mapper(configuration);

            var productsProvider = new ProductsProvider(dbContext, null, mapper);

            var product = await productsProvider.GetProductAsync(1);

            Assert.True(product.IsSuccess);
            Assert.NotNull(product.product);
            Assert.True(product.product.Id == 1);
            Assert.Null(product.ErrorMessage);
        }

        [Test]
        public async Task GetProductsReturnsProductUsingInValidID()
        {
            var options = new DbContextOptionsBuilder<ProductsDbContext>()
                .UseInMemoryDatabase(nameof(GetProductsReturnsAllProducts)).Options;
            var dbContext = new ProductsDbContext(options);

            CreateProducts(dbContext);

            var productsProfile = new ProductProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(productsProfile));
            var mapper = new Mapper(configuration);

            var productsProvider = new ProductsProvider(dbContext, null, mapper);

            var product = await productsProvider.GetProductAsync(-1);

            Assert.False(product.IsSuccess);
            Assert.Null(product.product);            
            Assert.NotNull(product.ErrorMessage);
        }

        private void CreateProducts(ProductsDbContext dbContext)
        {
            for (int i = 1; i <= 10; i++)
            {
                dbContext.Products.Add(new Product()
                {
                    Id = i,
                    Name = Guid.NewGuid().ToString(),
                    Inventory = i + 10,
                    Price = (decimal)(i * 3.14)
                });
            }

            dbContext.SaveChanges();
        }
    }
}
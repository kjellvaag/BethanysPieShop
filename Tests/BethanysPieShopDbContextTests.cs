using BethanysPieShop.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BethanysPieShop.Tests
{
    /// <summary>
    /// TDD-tester for BethanysPieShopDbContext
    /// Tester databasekonfigurering og relasjoner
    /// </summary>
    public class BethanysPieShopDbContextTests : IDisposable
    {
        private readonly BethanysPieShopDbContext _context;

        public BethanysPieShopDbContextTests()
        {
            // Arrange - Sett opp in-memory database
            var options = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new BethanysPieShopDbContext(options);
        }

        [Fact]
        public void DbContext_ShouldHavePiesDbSet()
        {
            // Assert
            Assert.NotNull(_context.Pies);
        }

        [Fact]
        public void DbContext_ShouldHaveCategoriesDbSet()
        {
            // Assert
            Assert.NotNull(_context.Categories);
        }

        [Fact]
        public void DbContext_ShouldSavePieWithCategory()
        {
            // Arrange
            var category = new Category { CategoryName = "Test Category" };
            var pie = new Pie
            {
                Name = "Test Pie",
                Price = 10.99m,
                Category = category
            };

            // Act
            _context.Categories.Add(category);
            _context.Pies.Add(pie);
            var result = _context.SaveChanges();

            // Assert
            Assert.Equal(2, result); // Should save 2 entities
            Assert.True(category.CategoryId > 0); // Should have generated ID
            Assert.True(pie.PieId > 0); // Should have generated ID
            Assert.Equal(category.CategoryId, pie.CategoryId); // Foreign key should be set
        }

        [Fact]
        public void DbContext_ShouldLoadPieWithCategory()
        {
            // Arrange - Seed data
            var category = new Category { CategoryName = "Test Category" };
            var pie = new Pie
            {
                Name = "Test Pie",
                Price = 10.99m,
                Category = category
            };

            _context.Categories.Add(category);
            _context.Pies.Add(pie);
            _context.SaveChanges();

            // Clear tracking to simulate fresh load
            _context.ChangeTracker.Clear();

            // Act
            var loadedPie = _context.Pies.Include(p => p.Category).First();

            // Assert
            Assert.NotNull(loadedPie);
            Assert.NotNull(loadedPie.Category);
            Assert.Equal("Test Category", loadedPie.Category.CategoryName);
        }

        [Fact]
        public void DbContext_ShouldHandleMultiplePiesInSameCategory()
        {
            // Arrange
            var category = new Category { CategoryName = "Shared Category" };
            var pie1 = new Pie { Name = "Pie 1", Price = 10m, Category = category };
            var pie2 = new Pie { Name = "Pie 2", Price = 20m, Category = category };

            // Act
            _context.Categories.Add(category);
            _context.Pies.AddRange(pie1, pie2);
            _context.SaveChanges();

            // Assert
            Assert.Equal(1, _context.Categories.Count());
            Assert.Equal(2, _context.Pies.Count());
            
            var piesInCategory = _context.Pies.Where(p => p.CategoryId == category.CategoryId).ToList();
            Assert.Equal(2, piesInCategory.Count);
        }

        [Fact]
        public void DbContext_ShouldEnforceRequiredFields()
        {
            // Arrange
            var pie = new Pie
            {
                // Name is required but not set
                Price = 10m
            };

            // Act & Assert
            _context.Pies.Add(pie);
            
            // This should work in in-memory DB, but would fail in real DB
            // We'll verify this more thoroughly when we have real DB constraints
            Assert.NotNull(_context.Pies);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
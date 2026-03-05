using BethanysPieShop.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BethanysPieShop.Tests
{
    /// <summary>
    /// TDD-tester for EF Core CategoryRepository
    /// Disse testene bruker In-Memory database for isolert testing
    /// </summary>
    public class EfCoreCategoryRepositoryTests : IDisposable
    {
        private readonly BethanysPieShopDbContext _context;
        private readonly CategoryRepository _repository;

        public EfCoreCategoryRepositoryTests()
        {
            // Arrange - Sett opp in-memory database for hver test
            var options = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unik database for hver test
                .Options;

            _context = new BethanysPieShopDbContext(options);
            _repository = new CategoryRepository(_context);

            // Seed testdata
            SeedTestData();
        }

        private void SeedTestData()
        {
            var categories = new List<Category>
            {
                new Category { CategoryId = 1, CategoryName = "Fruit pies" },
                new Category { CategoryId = 2, CategoryName = "Cheese cakes" },
                new Category { CategoryId = 3, CategoryName = "Chocolate pies" }
            };

            _context.Categories.AddRange(categories);
            _context.SaveChanges();
        }

        [Fact]
        public void AllCategories_ShouldReturnAllCategories()
        {
            // Act
            var categories = _repository.AllCategories.ToList();

            // Assert
            Assert.Equal(3, categories.Count);
            Assert.Contains(categories, c => c.CategoryName == "Fruit pies");
            Assert.Contains(categories, c => c.CategoryName == "Cheese cakes");
            Assert.Contains(categories, c => c.CategoryName == "Chocolate pies");
        }

        [Fact]
        public void AllCategories_ShouldReturnCategoriesInOrder()
        {
            // Act
            var categories = _repository.AllCategories.ToList();

            // Assert
            Assert.Equal("Fruit pies", categories[0].CategoryName);
            Assert.Equal("Cheese cakes", categories[1].CategoryName);
            Assert.Equal("Chocolate pies", categories[2].CategoryName);
        }

        [Fact]
        public void GetCategoryById_WithValidId_ShouldReturnCategory()
        {
            // Act
            var category = _repository.GetCategoryById(1);

            // Assert
            Assert.NotNull(category);
            Assert.Equal("Fruit pies", category.CategoryName);
        }

        [Fact]
        public void GetCategoryById_WithInvalidId_ShouldReturnNull()
        {
            // Act
            var category = _repository.GetCategoryById(999);

            // Assert
            Assert.Null(category);
        }

        [Fact]
        public void Repository_ShouldImplementICategoryRepositoryInterface()
        {
            // Assert
            Assert.IsAssignableFrom<ICategoryRepository>(_repository);
        }

        [Fact]
        public void AllCategories_ShouldBeQueryable()
        {
            // Act
            var categories = _repository.AllCategories;

            // Assert
            Assert.IsAssignableFrom<IEnumerable<Category>>(categories);
            
            // Should be able to query further
            var fruitPies = categories.Where(c => c.CategoryName.Contains("Fruit")).ToList();
            Assert.Single(fruitPies);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
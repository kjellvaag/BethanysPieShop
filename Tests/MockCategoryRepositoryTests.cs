using BethanysPieShop.Models;
using Xunit;

namespace BethanysPieShop.Tests
{
    public class MockCategoryRepositoryTests
    {
        [Fact]
        public void MockCategoryRepository_ShouldImplementICategoryRepository()
        {
            // Arrange & Act
            var repository = new MockCategoryRepository();
            
            // Assert
            Assert.IsAssignableFrom<ICategoryRepository>(repository);
        }
        
        [Fact]
        public void MockCategoryRepository_AllCategories_ShouldReturnTestData()
        {
            // Arrange
            var repository = new MockCategoryRepository();
            
            // Act
            var categories = repository.AllCategories.ToList();
            
            // Assert
            Assert.NotEmpty(categories);
            Assert.True(categories.Count >= 3); // Forventede kategorier fra originalen
            Assert.All(categories, category => 
            {
                Assert.True(category.CategoryId > 0);
                Assert.NotEmpty(category.CategoryName);
            });
        }
        
        [Fact]
        public void MockCategoryRepository_AllCategories_ShouldContainExpectedCategories()
        {
            // Arrange
            var repository = new MockCategoryRepository();
            
            // Act
            var categories = repository.AllCategories.ToList();
            
            // Assert
            Assert.Contains(categories, c => c.CategoryName.Contains("Fruit", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(categories, c => c.CategoryName.Contains("Cheese", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(categories, c => c.CategoryName.Contains("Seasonal", StringComparison.OrdinalIgnoreCase));
        }
        
        [Fact]
        public void MockCategoryRepository_AllCategories_ShouldHaveUniqueIds()
        {
            // Arrange
            var repository = new MockCategoryRepository();
            
            // Act
            var categories = repository.AllCategories.ToList();
            var categoryIds = categories.Select(c => c.CategoryId).ToList();
            
            // Assert
            Assert.Equal(categoryIds.Distinct().Count(), categoryIds.Count);
        }
    }
}

using BethanysPieShop.Models;
using Xunit;

namespace BethanysPieShop.Tests
{
    public class ICategoryRepositoryTests
    {
        [Fact]
        public void ICategoryRepository_ShouldHaveAllCategoriesProperty()
        {
            // Arrange - lager en mock implementation for testing av grensesnitt
            var mockRepo = new TestCategoryRepository();
            
            // Act
            var categories = mockRepo.AllCategories;
            
            // Assert
            Assert.NotNull(categories);
            Assert.IsAssignableFrom<IEnumerable<Category>>(categories);
        }
        
        [Fact]
        public void ICategoryRepository_AllCategories_ShouldReturnCategories()
        {
            // Arrange
            var mockRepo = new TestCategoryRepository();
            
            // Act
            var categories = mockRepo.AllCategories.ToList();
            
            // Assert
            Assert.NotEmpty(categories);
            Assert.All(categories, category => Assert.IsType<Category>(category));
        }
    }
    
    // Test implementation av ICategoryRepository for testing
    public class TestCategoryRepository : ICategoryRepository
    {
        public IEnumerable<Category> AllCategories => 
            new List<Category>
            {
                new Category { CategoryId = 1, CategoryName = "Test Category 1", Description = "Test Description 1" },
                new Category { CategoryId = 2, CategoryName = "Test Category 2", Description = "Test Description 2" }
            };
    }
}

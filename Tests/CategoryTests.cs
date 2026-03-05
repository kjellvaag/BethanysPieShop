using BethanysPieShop.Models;
using Xunit;

namespace BethanysPieShop.Tests
{
    public class CategoryTests
    {
        [Fact]
        public void Category_ShouldHaveRequiredProperties()
        {
            // Arrange & Act
            var category = new Category();
            
            // Assert - sjekke at alle nødvendige properties eksisterer
            Assert.NotNull(category);
            Assert.IsType<int>(category.CategoryId);
            Assert.IsType<string>(category.CategoryName);
            Assert.True(category.Description == null || category.Description is string);
            Assert.True(category.Pies == null || category.Pies is List<Pie>);
        }
        
        [Fact]
        public void Category_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var testId = 1;
            var testName = "Test Category";
            var testDescription = "Test Description";
            var testPies = new List<Pie>();
            
            // Act
            var category = new Category
            {
                CategoryId = testId,
                CategoryName = testName,
                Description = testDescription,
                Pies = testPies
            };
            
            // Assert
            Assert.Equal(testId, category.CategoryId);
            Assert.Equal(testName, category.CategoryName);
            Assert.Equal(testDescription, category.Description);
            Assert.Equal(testPies, category.Pies);
        }
        
        [Fact]
        public void Category_CategoryNameShouldDefaultToEmptyString()
        {
            // Arrange & Act
            var category = new Category();
            
            // Assert
            Assert.Equal(string.Empty, category.CategoryName);
        }
    }
}

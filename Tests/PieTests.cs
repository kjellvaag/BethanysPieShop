using BethanysPieShop.Models;
using Xunit;

namespace BethanysPieShop.Tests
{
    public class PieTests
    {
        [Fact]
        public void Pie_ShouldHaveRequiredProperties_WhenSet()
        {
            // Arrange
            var category = new Category { CategoryId = 1, CategoryName = "Test" };
            
            // Act
            var pie = new Pie { Category = category };
            
            // Assert - sjekke at alle nødvendige properties eksisterer
            Assert.NotNull(pie);
            Assert.IsType<int>(pie.PieId);
            Assert.IsType<string>(pie.Name);
            Assert.True(pie.ShortDescription == null || pie.ShortDescription is string);
            Assert.True(pie.LongDescription == null || pie.LongDescription is string);
            Assert.True(pie.AllergyInformation == null || pie.AllergyInformation is string);
            Assert.IsType<decimal>(pie.Price);
            Assert.True(pie.ImageUrl == null || pie.ImageUrl is string);
            Assert.True(pie.ImageThumbnailUrl == null || pie.ImageThumbnailUrl is string);
            Assert.IsType<bool>(pie.IsPieOfTheWeek);
            Assert.IsType<bool>(pie.InStock);
            Assert.IsType<int>(pie.CategoryId);
            Assert.IsType<Category>(pie.Category);
        }
        
        [Fact]
        public void Pie_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var testId = 1;
            var testName = "Test Pie";
            var testShortDesc = "Short description";
            var testLongDesc = "Long description";
            var testAllergyInfo = "Contains nuts";
            var testPrice = 15.95m;
            var testImageUrl = "http://example.com/pie.jpg";
            var testThumbnailUrl = "http://example.com/pie_thumb.jpg";
            var testIsPieOfWeek = true;
            var testInStock = false;
            var testCategoryId = 2;
            var testCategory = new Category { CategoryId = 2, CategoryName = "Test Category" };
            
            // Act
            var pie = new Pie
            {
                PieId = testId,
                Name = testName,
                ShortDescription = testShortDesc,
                LongDescription = testLongDesc,
                AllergyInformation = testAllergyInfo,
                Price = testPrice,
                ImageUrl = testImageUrl,
                ImageThumbnailUrl = testThumbnailUrl,
                IsPieOfTheWeek = testIsPieOfWeek,
                InStock = testInStock,
                CategoryId = testCategoryId,
                Category = testCategory
            };
            
            // Assert
            Assert.Equal(testId, pie.PieId);
            Assert.Equal(testName, pie.Name);
            Assert.Equal(testShortDesc, pie.ShortDescription);
            Assert.Equal(testLongDesc, pie.LongDescription);
            Assert.Equal(testAllergyInfo, pie.AllergyInformation);
            Assert.Equal(testPrice, pie.Price);
            Assert.Equal(testImageUrl, pie.ImageUrl);
            Assert.Equal(testThumbnailUrl, pie.ImageThumbnailUrl);
            Assert.Equal(testIsPieOfWeek, pie.IsPieOfTheWeek);
            Assert.Equal(testInStock, pie.InStock);
            Assert.Equal(testCategoryId, pie.CategoryId);
            Assert.Equal(testCategory, pie.Category);
        }
        
        [Fact]
        public void Pie_NameShouldDefaultToEmptyString()
        {
            // Arrange & Act
            var pie = new Pie();
            
            // Assert
            Assert.Equal(string.Empty, pie.Name);
        }
        
        [Fact]
        public void Pie_DefaultValuesShouldBeCorrect()
        {
            // Arrange & Act
            var pie = new Pie();
            
            // Assert
            Assert.Equal(0, pie.PieId);
            Assert.Equal(0m, pie.Price);
            Assert.False(pie.IsPieOfTheWeek);
            Assert.False(pie.InStock);
            Assert.Equal(0, pie.CategoryId);
            // Category er null ved default initialization (default! advarer bare compile time)
        }
    }
}

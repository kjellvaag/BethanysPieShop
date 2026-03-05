using BethanysPieShop.Models;
using Xunit;

namespace BethanysPieShop.Tests
{
    public class MockPieRepositoryTests
    {
        [Fact]
        public void MockPieRepository_ShouldImplementIPieRepository()
        {
            // Arrange & Act
            var repository = new MockPieRepository();
            
            // Assert
            Assert.IsAssignableFrom<IPieRepository>(repository);
        }
        
        [Fact]
        public void MockPieRepository_AllPies_ShouldReturnTestData()
        {
            // Arrange
            var repository = new MockPieRepository();
            
            // Act
            var pies = repository.AllPies.ToList();
            
            // Assert
            Assert.NotEmpty(pies);
            Assert.True(pies.Count >= 4); // Forventede pies fra originalen
            Assert.All(pies, pie => 
            {
                Assert.True(pie.PieId > 0);
                Assert.NotEmpty(pie.Name);
                Assert.True(pie.Price > 0);
                Assert.NotNull(pie.Category);
            });
        }
        
        [Fact]
        public void MockPieRepository_PiesOfTheWeek_ShouldOnlyReturnPiesOfTheWeek()
        {
            // Arrange
            var repository = new MockPieRepository();
            
            // Act
            var piesOfTheWeek = repository.PiesOfTheWeek.ToList();
            
            // Assert
            Assert.NotEmpty(piesOfTheWeek);
            Assert.All(piesOfTheWeek, pie => Assert.True(pie.IsPieOfTheWeek));
        }
        
        [Fact]
        public void MockPieRepository_GetPieById_ShouldReturnCorrectPie()
        {
            // Arrange
            var repository = new MockPieRepository();
            var expectedId = 1;
            
            // Act
            var pie = repository.GetPieById(expectedId);
            
            // Assert
            Assert.NotNull(pie);
            Assert.Equal(expectedId, pie.PieId);
        }
        
        [Fact]
        public void MockPieRepository_GetPieById_ShouldReturnNullForInvalidId()
        {
            // Arrange
            var repository = new MockPieRepository();
            var invalidId = 999;
            
            // Act
            var pie = repository.GetPieById(invalidId);
            
            // Assert
            Assert.Null(pie);
        }
        
        [Fact]
        public void MockPieRepository_SearchPies_ShouldReturnMatchingPies()
        {
            // Arrange
            var repository = new MockPieRepository();
            var searchQuery = "Strawberry";
            
            // Act
            var results = repository.SearchPies(searchQuery).ToList();
            
            // Assert
            Assert.NotEmpty(results);
            Assert.All(results, pie => 
                Assert.Contains(searchQuery, pie.Name, StringComparison.OrdinalIgnoreCase));
        }
        
        [Fact]
        public void MockPieRepository_SearchPies_ShouldReturnEmptyForNoMatches()
        {
            // Arrange
            var repository = new MockPieRepository();
            var searchQuery = "NonExistentPie";
            
            // Act
            var results = repository.SearchPies(searchQuery).ToList();
            
            // Assert
            Assert.Empty(results);
        }
        
        [Fact]
        public void MockPieRepository_AllPies_ShouldHaveValidCategories()
        {
            // Arrange
            var repository = new MockPieRepository();
            
            // Act
            var pies = repository.AllPies.ToList();
            
            // Assert
            Assert.All(pies, pie => 
            {
                Assert.NotNull(pie.Category);
                Assert.True(pie.CategoryId > 0);
                Assert.Equal(pie.CategoryId, pie.Category.CategoryId);
            });
        }
    }
}

using BethanysPieShop.Models;
using Xunit;

namespace BethanysPieShop.Tests
{
    public class IPieRepositoryTests
    {
        [Fact]
        public void IPieRepository_ShouldHaveAllPiesProperty()
        {
            // Arrange
            var mockRepo = new TestPieRepository();
            
            // Act
            var pies = mockRepo.AllPies;
            
            // Assert
            Assert.NotNull(pies);
            Assert.IsAssignableFrom<IEnumerable<Pie>>(pies);
        }
        
        [Fact]
        public void IPieRepository_ShouldHavePiesOfTheWeekProperty()
        {
            // Arrange
            var mockRepo = new TestPieRepository();
            
            // Act
            var piesOfTheWeek = mockRepo.PiesOfTheWeek;
            
            // Assert
            Assert.NotNull(piesOfTheWeek);
            Assert.IsAssignableFrom<IEnumerable<Pie>>(piesOfTheWeek);
        }
        
        [Fact]
        public void IPieRepository_GetPieById_ShouldReturnPie()
        {
            // Arrange
            var mockRepo = new TestPieRepository();
            var existingId = 1;
            
            // Act
            var pie = mockRepo.GetPieById(existingId);
            
            // Assert
            Assert.NotNull(pie);
            Assert.IsType<Pie>(pie);
            Assert.Equal(existingId, pie.PieId);
        }
        
        [Fact]
        public void IPieRepository_GetPieById_ShouldReturnNullForNonExistentId()
        {
            // Arrange
            var mockRepo = new TestPieRepository();
            var nonExistentId = 999;
            
            // Act
            var pie = mockRepo.GetPieById(nonExistentId);
            
            // Assert
            Assert.Null(pie);
        }
        
        [Fact]
        public void IPieRepository_SearchPies_ShouldReturnMatchingPies()
        {
            // Arrange
            var mockRepo = new TestPieRepository();
            var searchQuery = "Apple";
            
            // Act
            var searchResults = mockRepo.SearchPies(searchQuery);
            
            // Assert
            Assert.NotNull(searchResults);
            Assert.IsAssignableFrom<IEnumerable<Pie>>(searchResults);
        }
        
        [Fact]
        public void IPieRepository_PiesOfTheWeek_ShouldOnlyReturnPiesOfTheWeek()
        {
            // Arrange
            var mockRepo = new TestPieRepository();
            
            // Act
            var piesOfTheWeek = mockRepo.PiesOfTheWeek.ToList();
            
            // Assert
            Assert.All(piesOfTheWeek, pie => Assert.True(pie.IsPieOfTheWeek));
        }
    }
    
    // Test implementation av IPieRepository for testing
    public class TestPieRepository : IPieRepository
    {
        private readonly List<Pie> _pies;
        
        public TestPieRepository()
        {
            var category = new Category { CategoryId = 1, CategoryName = "Test Category" };
            
            _pies = new List<Pie>
            {
                new Pie
                {
                    PieId = 1,
                    Name = "Apple Pie",
                    Price = 15.95m,
                    IsPieOfTheWeek = true,
                    InStock = true,
                    Category = category
                },
                new Pie
                {
                    PieId = 2,
                    Name = "Blueberry Pie",
                    Price = 18.95m,
                    IsPieOfTheWeek = false,
                    InStock = true,
                    Category = category
                },
                new Pie
                {
                    PieId = 3,
                    Name = "Apple Crumble",
                    Price = 12.95m,
                    IsPieOfTheWeek = true,
                    InStock = false,
                    Category = category
                }
            };
        }
        
        public IEnumerable<Pie> AllPies => _pies;
        
        public IEnumerable<Pie> PiesOfTheWeek => _pies.Where(p => p.IsPieOfTheWeek);
        
        public Pie? GetPieById(int pieId) => _pies.FirstOrDefault(p => p.PieId == pieId);
        
        public IEnumerable<Pie> SearchPies(string searchQuery) => 
            _pies.Where(p => p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
    }
}

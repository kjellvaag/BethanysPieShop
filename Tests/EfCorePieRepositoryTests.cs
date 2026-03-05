using BethanysPieShop.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BethanysPieShop.Tests
{
    /// <summary>
    /// TDD-tester for EF Core PieRepository
    /// Disse testene bruker In-Memory database for isolert testing
    /// </summary>
    public class EfCorePieRepositoryTests : IDisposable
    {
        private readonly BethanysPieShopDbContext _context;
        private readonly PieRepository _repository;

        public EfCorePieRepositoryTests()
        {
            // Arrange - Sett opp in-memory database for hver test
            var options = new DbContextOptionsBuilder<BethanysPieShopDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unik database for hver test
                .Options;

            _context = new BethanysPieShopDbContext(options);
            _repository = new PieRepository(_context);

            // Seed testdata
            SeedTestData();
        }

        private void SeedTestData()
        {
            var category1 = new Category { CategoryId = 1, CategoryName = "Fruit pies" };
            var category2 = new Category { CategoryId = 2, CategoryName = "Cheese cakes" };

            _context.Categories.AddRange(category1, category2);

            var pies = new List<Pie>
            {
                new Pie
                {
                    PieId = 1,
                    Name = "Apple Pie",
                    Price = 15.95m,
                    IsPieOfTheWeek = true,
                    InStock = true,
                    CategoryId = 1,
                    Category = category1
                },
                new Pie
                {
                    PieId = 2,
                    Name = "Blueberry Pie", 
                    Price = 18.95m,
                    IsPieOfTheWeek = false,
                    InStock = true,
                    CategoryId = 1,
                    Category = category1
                },
                new Pie
                {
                    PieId = 3,
                    Name = "Cheesecake",
                    Price = 22.95m,
                    IsPieOfTheWeek = true,
                    InStock = false,
                    CategoryId = 2,
                    Category = category2
                }
            };

            _context.Pies.AddRange(pies);
            _context.SaveChanges();
        }

        [Fact]
        public void AllPies_ShouldReturnAllPiesWithCategories()
        {
            // Act
            var pies = _repository.AllPies.ToList();

            // Assert
            Assert.Equal(3, pies.Count);
            Assert.All(pies, pie => Assert.NotNull(pie.Category)); // Verifiser at Category er loaded via Include
        }

        [Fact]
        public void PiesOfTheWeek_ShouldReturnOnlyPiesOfTheWeek()
        {
            // Act
            var piesOfTheWeek = _repository.PiesOfTheWeek.ToList();

            // Assert
            Assert.Equal(2, piesOfTheWeek.Count); // Apple Pie og Cheesecake
            Assert.All(piesOfTheWeek, pie => Assert.True(pie.IsPieOfTheWeek));
        }

        [Fact]
        public void GetPieById_WithValidId_ShouldReturnPieWithCategory()
        {
            // Act
            var pie = _repository.GetPieById(1);

            // Assert
            Assert.NotNull(pie);
            Assert.Equal("Apple Pie", pie.Name);
            Assert.NotNull(pie.Category);
            Assert.Equal("Fruit pies", pie.Category.CategoryName);
        }

        [Fact]
        public void GetPieById_WithInvalidId_ShouldReturnNull()
        {
            // Act
            var pie = _repository.GetPieById(999);

            // Assert
            Assert.Null(pie);
        }

        [Fact]
        public void SearchPies_WithValidSearchTerm_ShouldReturnMatchingPies()
        {
            // Act
            var searchResults = _repository.SearchPies("Apple");

            // Assert
            var results = searchResults.ToList();
            Assert.Single(results);
            Assert.Equal("Apple Pie", results[0].Name);
        }

        [Fact]
        public void SearchPies_WithPartialMatch_ShouldReturnMatchingPies()
        {
            // Act
            var searchResults = _repository.SearchPies("Pie");

            // Assert
            var results = searchResults.ToList();
            Assert.Equal(2, results.Count); // Apple Pie og Blueberry Pie
        }

        [Fact]
        public void SearchPies_CaseInsensitive_ShouldReturnMatchingPies()
        {
            // Act
            var searchResults = _repository.SearchPies("APPLE");

            // Assert
            var results = searchResults.ToList();
            Assert.Single(results);
            Assert.Equal("Apple Pie", results[0].Name);
        }

        [Fact]
        public void SearchPies_WithNoMatches_ShouldReturnEmpty()
        {
            // Act
            var searchResults = _repository.SearchPies("NonExistent");

            // Assert
            Assert.Empty(searchResults);
        }

        [Fact]
        public void SearchPies_WithEmptyString_ShouldReturnAllPies()
        {
            // Act
            var searchResults = _repository.SearchPies("");

            // Assert
            Assert.Equal(3, searchResults.Count());
        }

        [Fact]
        public void Repository_ShouldImplementIPieRepositoryInterface()
        {
            // Assert
            Assert.IsAssignableFrom<IPieRepository>(_repository);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
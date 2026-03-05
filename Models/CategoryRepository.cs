using Microsoft.EntityFrameworkCore;

namespace BethanysPieShop.Models
{
    /// <summary>
    /// EF Core implementasjon av ICategoryRepository
    /// Erstatter MockCategoryRepository med ekte databaseoperasjoner
    /// </summary>
    public class CategoryRepository : ICategoryRepository
    {
        private readonly BethanysPieShopDbContext _context;

        public CategoryRepository(BethanysPieShopDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Henter alle kategorier sortert alfabetisk etter navn
        /// </summary>
        public IEnumerable<Category> AllCategories => 
            _context.Categories.OrderBy(c => c.CategoryId);

        /// <summary>
        /// Henter en spesifikk kategori basert på ID
        /// Returnerer null hvis kategori ikke finnes
        /// </summary>
        public Category? GetCategoryById(int categoryId) => 
            _context.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
    }
}
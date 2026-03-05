using Microsoft.EntityFrameworkCore;

namespace BethanysPieShop.Models
{
    /// <summary>
    /// EF Core implementasjon av IPieRepository
    /// Erstatter MockPieRepository med ekte databaseoperasjoner
    /// </summary>
    public class PieRepository : IPieRepository
    {
        private readonly BethanysPieShopDbContext _context;

        public PieRepository(BethanysPieShopDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Henter alle pies med tilhørende Category-informasjon
        /// Bruker Include() for å laste relaterte data (Eager Loading)
        /// </summary>
        public IEnumerable<Pie> AllPies => 
            _context.Pies.Include(p => p.Category).OrderBy(p => p.Name);

        /// <summary>
        /// Henter kun pies som er "Pie of the Week" med Category-informasjon
        /// </summary>
        public IEnumerable<Pie> PiesOfTheWeek => 
            _context.Pies.Include(p => p.Category)
                         .Where(p => p.IsPieOfTheWeek)
                         .OrderBy(p => p.Name);

        /// <summary>
        /// Henter en spesifikk pie basert på ID
        /// Returnerer null hvis pie ikke finnes
        /// </summary>
        public Pie? GetPieById(int pieId) => 
            _context.Pies.Include(p => p.Category)
                         .FirstOrDefault(p => p.PieId == pieId);

        /// <summary>
        /// Søker etter pies basert på navn (case-insensitive)
        /// Returnerer tom liste hvis ingen matches
        /// </summary>
        public IEnumerable<Pie> SearchPies(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return AllPies;
            }

            return _context.Pies.Include(p => p.Category)
                                .Where(p => p.Name.ToLower().Contains(searchQuery.ToLower()))
                                .OrderBy(p => p.Name);
        }
    }
}
namespace BethanysPieShop.Models
{
    public class MockPieRepository : IPieRepository
    {
        private readonly ICategoryRepository _categoryRepository = new MockCategoryRepository();

        public IEnumerable<Pie> AllPies =>
            new List<Pie>
            {
                new Pie {PieId = 1, Name="Strawberry Pie", Price=15.95M, ShortDescription="Lorem Ipsum", LongDescription="Icing carrot cake jelly-o cheesecake. Sweet roll marzipan marshmallow toffee brownie brownie candy tootsie roll. Chocolate cake gingerbread tootsie roll oat cake pie chocolate bar cookie dragée brownie. Lollipop cotton candy cake bear claw oat cake.", CategoryId=1, Category = _categoryRepository.AllCategories.ToList()[0],ImageUrl="~/images/carousel1.jpg", InStock=true, IsPieOfTheWeek=false, ImageThumbnailUrl="~/images/carousel1.jpg"},
                new Pie {PieId = 2, Name="Cheese cake", Price=18.95M, ShortDescription="Lorem Ipsum", LongDescription="Icing carrot cake jelly-o cheesecake. Sweet roll marzipan marshmallow toffee brownie brownie candy tootsie roll. Chocolate cake gingerbread tootsie roll oat cake pie chocolate bar cookie dragée brownie.", CategoryId=2, Category = _categoryRepository.AllCategories.ToList()[1],ImageUrl="~/images/carousel2.jpg", InStock=true, IsPieOfTheWeek=false, ImageThumbnailUrl="~/images/carousel2.jpg"},
                new Pie {PieId = 3, Name="Rhubarb Pie", Price=15.95M, ShortDescription="Lorem Ipsum", LongDescription="Icing carrot cake jelly-o cheesecake. Sweet roll marzipan marshmallow toffee brownie brownie candy tootsie roll. Chocolate cake gingerbread tootsie roll oat cake pie chocolate bar cookie dragée brownie.", CategoryId=1, Category = _categoryRepository.AllCategories.ToList()[0],ImageUrl="~/images/carousel3.jpg", InStock=true, IsPieOfTheWeek=true, ImageThumbnailUrl="~/images/carousel3.jpg"},
                new Pie {PieId = 4, Name="Pumpkin Pie", Price=12.95M, ShortDescription="Lorem Ipsum", LongDescription="Icing carrot cake jelly-o cheesecake. Sweet roll marzipan marshmallow toffee brownie brownie candy tootsie roll. Chocolate cake gingerbread tootsie roll oat cake pie chocolate bar cookie dragée brownie.", CategoryId=3, Category = _categoryRepository.AllCategories.ToList()[2],ImageUrl="~/images/bethanys-pie-shop-logomark.png", InStock=true, IsPieOfTheWeek=true, ImageThumbnailUrl="~/images/bethanys-pie-shop-logomark.png"}
            };
        public IEnumerable<Pie> PiesOfTheWeek
        {
            get
            {
                return AllPies.Where(p => p.IsPieOfTheWeek);
            }
        }

        public Pie? GetPieById(int pieId) => AllPies.FirstOrDefault(p => p.PieId == pieId);

        public IEnumerable<Pie> SearchPies(string searchQuery)
        {
            return AllPies.Where(p => p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
        }
    }
}

namespace BethanysPieShop.Models
{
    /// <summary>
    /// Statisk klasse for å initialisere database med seed-data
    /// Separerer data-seeding fra migrasjoner for bedre kontroll
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Seeder databasen med initial data hvis den er tom
        /// Kalles fra Program.cs ved oppstart
        /// </summary>
        /// <param name="context">Database context</param>
        public static void Seed(BethanysPieShopDbContext context)
        {
            // Sørg for at databasen er opprettet
            context.Database.EnsureCreated();

            // Sjekk om vi allerede har data (unngå duplikater)
            if (context.Categories.Any())
            {
                return; // Database er allerede seeded
            }

            // Seed kategorier
            var fruitPiesCategory = new Category 
            { 
                CategoryName = "Fruit pies", 
                Description = "Delicious fruit-based pies" 
            };
            var cheeseCakesCategory = new Category 
            { 
                CategoryName = "Cheese cakes", 
                Description = "Rich and creamy cheesecakes" 
            };
            var seasonalPiesCategory = new Category 
            { 
                CategoryName = "Seasonal pies", 
                Description = "Pies that match the current season" 
            };

            context.Categories.AddRange(fruitPiesCategory, cheeseCakesCategory, seasonalPiesCategory);
            context.SaveChanges();

            // Seed pies (må gjøres etter kategorier pga foreign key)
            var pies = new Pie[]
            {
                new Pie
                {
                    Name = "Apple Pie",
                    Price = 12.95M,
                    ShortDescription = "Our famous apple pie",
                    LongDescription = "Icing carrot cake jelly-o cheesecake. Sweet roll marzipan marshmallow toffee brownie brownie candy tootsie roll. Chocolate cake gingerbread tootsie roll oat cake pie chocolate bar cookie dragée brownie.",
                    CategoryId = fruitPiesCategory.CategoryId, // Bruk generert ID
                    ImageUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/applepie.jpg",
                    InStock = true,
                    IsPieOfTheWeek = true,
                    ImageThumbnailUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/applepiesmall.jpg"
                },
                new Pie
                {
                    Name = "Blueberry Cheese Cake",
                    Price = 18.95M,
                    ShortDescription = "You'll love it!",
                    LongDescription = "Icing carrot cake jelly-o cheesecake. Sweet roll marzipan marshmallow toffee brownie brownie candy tootsie roll. Chocolate cake gingerbread tootsie roll oat cake pie chocolate bar cookie dragée brownie.",
                    CategoryId = cheeseCakesCategory.CategoryId, // Bruk generert ID
                    ImageUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/blueberrycheesecake.jpg",
                    InStock = true,
                    IsPieOfTheWeek = false,
                    ImageThumbnailUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/blueberrycheesecakesmall.jpg"
                },
                new Pie
                {
                    Name = "Cheese Cake",
                    Price = 18.95M,
                    ShortDescription = "Plain cheese cake. Plain pleasure.",
                    LongDescription = "Icing carrot cake jelly-o cheesecake. Sweet roll marzipan marshmallow toffee brownie brownie candy tootsie roll. Chocolate cake gingerbread tootsie roll oat cake pie chocolate bar cookie dragée brownie.",
                    CategoryId = cheeseCakesCategory.CategoryId, // Bruk generert ID
                    ImageUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/cheesecake.jpg",
                    InStock = true,
                    IsPieOfTheWeek = false,
                    ImageThumbnailUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/cheesecakesmall.jpg"
                },
                new Pie
                {
                    Name = "Cherry Pie",
                    Price = 15.95M,
                    ShortDescription = "A summer classic!",
                    LongDescription = "Icing carrot cake jelly-o cheesecake. Sweet roll marzipan marshmallow toffee brownie brownie candy tootsie roll. Chocolate cake gingerbread tootsie roll oat cake pie chocolate bar cookie dragée brownie.",
                    CategoryId = fruitPiesCategory.CategoryId, // Bruk generert ID
                    ImageUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/cherrypie.jpg",
                    InStock = true,
                    IsPieOfTheWeek = false,
                    ImageThumbnailUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/cherrypiesmall.jpg"
                },
                new Pie
                {
                    Name = "Christmas Apple Pie",
                    Price = 13.95M,
                    ShortDescription = "Happy holidays with this pie!",
                    LongDescription = "Icing carrot cake jelly-o cheesecake. Sweet roll marzipan marshmallow toffee brownie brownie candy tootsie roll. Chocolate cake gingerbread tootsie roll oat cake pie chocolate bar cookie dragée brownie.",
                    CategoryId = seasonalPiesCategory.CategoryId, // Bruk generert ID
                    ImageUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/christmasapplepie.jpg",
                    InStock = true,
                    IsPieOfTheWeek = false,
                    ImageThumbnailUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/christmasapplepiesmall.jpg"
                }
            };

            context.Pies.AddRange(pies);
            context.SaveChanges();
        }
    }
}
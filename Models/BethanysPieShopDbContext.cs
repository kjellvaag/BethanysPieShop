using Microsoft.EntityFrameworkCore;

namespace BethanysPieShop.Models
{
    /// <summary>
    /// DbContext for Bethany's Pie Shop - håndterer databaseforbindelse og operasjoner
    /// </summary>
    public class BethanysPieShopDbContext : DbContext
    {
        public BethanysPieShopDbContext(DbContextOptions<BethanysPieShopDbContext> options) : base(options)
        {
        }

        // DbSet representerer tabeller i databasen
        public DbSet<Pie> Pies { get; set; }
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Konfigurerer modell-relasjoner og database-constraints
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurer Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.CategoryId);
                entity.Property(c => c.CategoryName)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(c => c.Description)
                    .HasMaxLength(500);
            });

            // Konfigurer Pie entity
            modelBuilder.Entity<Pie>(entity =>
            {
                entity.HasKey(p => p.PieId);
                
                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(p => p.ShortDescription)
                    .HasMaxLength(200);
                
                entity.Property(p => p.LongDescription)
                    .HasMaxLength(1000);
                
                entity.Property(p => p.AllergyInformation)
                    .HasMaxLength(300);
                
                entity.Property(p => p.Price)
                    .HasColumnType("decimal(18,2)");
                
                entity.Property(p => p.ImageUrl)
                    .HasMaxLength(500);
                
                entity.Property(p => p.ImageThumbnailUrl)
                    .HasMaxLength(500);

                // Konfigurer relasjon mellom Pie og Category (One-to-Many)
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Pies)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict); // Forhindrer sletting av Category hvis den har Pies
            });

            // Seed initial data for utviklingsformål
            SeedData(modelBuilder);
        }

        /// <summary>
        /// Seeder initial data til databasen
        /// </summary>
        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category 
                { 
                    CategoryId = 1, 
                    CategoryName = "Fruit pies", 
                    Description = "Delicious fruit-based pies" 
                },
                new Category 
                { 
                    CategoryId = 2, 
                    CategoryName = "Cheese cakes", 
                    Description = "Rich and creamy cheesecakes" 
                },
                new Category 
                { 
                    CategoryId = 3, 
                    CategoryName = "Seasonal pies", 
                    Description = "Pies that match the current season" 
                }
            );

            // Seed Pies
            modelBuilder.Entity<Pie>().HasData(
                new Pie
                {
                    PieId = 1,
                    Name = "Apple Pie",
                    Price = 12.95M,
                    ShortDescription = "Our famous apple pie!",
                    LongDescription = "Icing carrot cake jelly-o cheesecake. Sweet roll marzipan marshmallow toffee brownie brownie candy tootsie roll. Chocolate cake gingerbread tootsie roll oat cake pie chocolate bar cookie dragée brownie. Lollipop cotton candy cake bear claw oat cake. Dragée candy canes dessert tart. Marzipan dragée gummies lollipop jujubes chocolate bar candy canes. Icing gingerbread chupa chups cotton candy cookie sweet icing bonbon gummies. Gummies lollipop brownie biscuit danish chocolate cake. Danish powder cookie macaroon chocolate donut tart. Carrot cake dragée croissant lemon drops liquorice lemon drops cookie lollipop tart. Carrot cake carrot cake liquorice sugar plum topping bonbon pie muffin jujubes. Jelly pastry wafer tart caramels bear claw. Tiramisu tart pie cake danish lemon drops. Brownie cupcake dragée gummies.",
                    CategoryId = 1,
                    ImageUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/applepie.jpg",
                    InStock = true,
                    IsPieOfTheWeek = true,
                    ImageThumbnailUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/applepiesmall.jpg",
                    AllergyInformation = ""
                },
                new Pie
                {
                    PieId = 2,
                    Name = "Blueberry Cheese Cake",
                    Price = 18.95M,
                    ShortDescription = "You'll love it!",
                    LongDescription = "Icing carrot cake jelly-o cheesecake. Sweet roll marzipan marshmallow toffee brownie brownie candy tootsie roll. Chocolate cake gingerbread tootsie roll oat cake pie chocolate bar cookie dragée brownie. Lollipop cotton candy cake bear claw oat cake. Dragée candy canes dessert tart. Marzipan dragée gummies lollipop jujubes chocolate bar candy canes. Icing gingerbread chupa chups cotton candy cookie sweet icing bonbon gummies. Gummies lollipop brownie biscuit danish chocolate cake. Danish powder cookie macaroon chocolate donut tart. Carrot cake dragée croissant lemon drops liquorice lemon drops cookie lollipop tart. Carrot cake carrot cake liquorice sugar plum topping bonbon pie muffin jujubes. Jelly pastry wafer tart caramels bear claw. Tiramisu tart pie cake danish lemon drops. Brownie cupcake dragée gummies.",
                    CategoryId = 2,
                    ImageUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/blueberrycheesecake.jpg",
                    InStock = true,
                    IsPieOfTheWeek = false,
                    ImageThumbnailUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/blueberrycheesecakesmall.jpg",
                    AllergyInformation = ""
                },
                new Pie
                {
                    PieId = 3,
                    Name = "Cheese Cake",
                    Price = 18.95M,
                    ShortDescription = "Plain cheese cake. Plain pleasure.",
                    LongDescription = "Icing carrot cake jelly-o cheesecake. Sweet roll marzipan marshmallow toffee brownie brownie candy tootsie roll. Chocolate cake gingerbread tootsie roll oat cake pie chocolate bar cookie dragée brownie. Lollipop cotton candy cake bear claw oat cake. Dragée candy canes dessert tart. Marzipan dragée gummies lollipop jujubes chocolate bar candy canes. Icing gingerbread chupa chups cotton candy cookie sweet icing bonbon gummies. Gummies lollipop brownie biscuit danish chocolate cake. Danish powder cookie macaroon chocolate donut tart. Carrot cake dragée croissant lemon drops liquorice lemon drops cookie lollipop tart. Carrot cake carrot cake liquorice sugar plum topping bonbon pie muffin jujubes. Jelly pastry wafer tart caramels bear claw. Tiramisu tart pie cake danish lemon drops. Brownie cupcake dragée gummies.",
                    CategoryId = 2,
                    ImageUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/cheesecake.jpg",
                    InStock = true,
                    IsPieOfTheWeek = false,
                    ImageThumbnailUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/cheesecakesmall.jpg",
                    AllergyInformation = ""
                },
                new Pie
                {
                    PieId = 4,
                    Name = "Cherry Pie",
                    Price = 15.95M,
                    ShortDescription = "A summer classic!",
                    LongDescription = "Icing carrot cake jelly-o cheesecake. Sweet roll marzipan marshmallow toffee brownie brownie candy tootsie roll. Chocolate cake gingerbread tootsie roll oat cake pie chocolate bar cookie dragée brownie. Lollipop cotton candy cake bear claw oat cake. Dragée candy canes dessert tart. Marzipan dragée gummies lollipop jujubes chocolate bar candy canes. Icing gingerbread chupa chups cotton candy cookie sweet icing bonbon gummies. Gummies lollipop brownie biscuit danish chocolate cake. Danish powder cookie macaroon chocolate donut tart. Carrot cake dragée croissant lemon drops liquorice lemon drops cookie lollipop tart. Carrot cake carrot cake liquorice sugar plum topping bonbon pie muffin jujubes. Jelly pastry wafer tart caramels bear claw. Tiramisu tart pie cake danish lemon drops. Brownie cupcake dragée gummies.",
                    CategoryId = 1,
                    ImageUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/cherrypie.jpg",
                    InStock = true,
                    IsPieOfTheWeek = true,
                    ImageThumbnailUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/cherrypiesmall.jpg",
                    AllergyInformation = ""
                },
                new Pie
                {
                    PieId = 5,
                    Name = "Christmas Apple Pie",
                    Price = 13.95M,
                    ShortDescription = "Happy holidays with this pie!",
                    LongDescription = "Icing carrot cake jelly-o cheesecake. Sweet roll marzipan marshmallow toffee brownie brownie candy tootsie roll. Chocolate cake gingerbread tootsie roll oat cake pie chocolate bar cookie dragée brownie. Lollipop cotton candy cake bear claw oat cake. Dragée candy canes dessert tart. Marzipan dragée gummies lollipop jujubes chocolate bar candy canes. Icing gingerbread chupa chups cotton candy cookie sweet icing bonbon gummies. Gummies lollipop brownie biscuit danish chocolate cake. Danish powder cookie macaroon chocolate donut tart. Carrot cake dragée croissant lemon drops liquorice lemon drops cookie lollipop tart. Carrot cake carrot cake liquorice sugar plum topping bonbon pie muffin jujubes. Jelly pastry wafer tart caramels bear claw. Tiramisu tart pie cake danish lemon drops. Brownie cupcake dragée gummies.",
                    CategoryId = 3,
                    ImageUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/christmasapplepie.jpg",
                    InStock = true,
                    IsPieOfTheWeek = false,
                    ImageThumbnailUrl = "https://gillcleerenpluralsight.blob.core.windows.net/files/christmasapplepiesmall.jpg",
                    AllergyInformation = ""
                }
            );
        }
    }
}
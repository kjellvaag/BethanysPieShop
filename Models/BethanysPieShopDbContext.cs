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

        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

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

        }
    }
}
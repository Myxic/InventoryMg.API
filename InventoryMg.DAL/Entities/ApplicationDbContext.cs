using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryMg.DAL.Entities
{
    public class ApplicationDbContext : IdentityDbContext<UserProfile, AppRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<Product>()
                .Property(p => p.ProductImagePath)
                .IsRequired(false)
                .HasMaxLength(500);

            base.OnModelCreating(builder);
        }


    }
}

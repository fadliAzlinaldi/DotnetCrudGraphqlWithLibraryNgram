using DotnetGraphQLCRUD.Model;
using Microsoft.EntityFrameworkCore;

namespace DotnetGraphQLCRUD
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Product>()
                .Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .Property(x => x.Description)
                .HasMaxLength(100);

            modelBuilder.Entity<Product>()
                .Property(x => x.Category)
                .HasMaxLength(50)
                .IsRequired();
        }
    }


}

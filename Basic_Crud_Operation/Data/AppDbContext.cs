using Basic_Crud_Operation.Models;
using Microsoft.EntityFrameworkCore;

namespace Basic_Crud_Operation.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Automatically finds every IEntityTypeConfiguration<T> in this project
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}

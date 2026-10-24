using Basic_Crud_Operation.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Basic_Crud_Operation.ModelConfiguration
{
    public class ProductConfiguration: IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Price)
                   .HasColumnType("decimal(18,2)");

            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");   // SQL Server fills this automatically

            builder.HasData(
                    new Product { Id = 1, Name = "Keyboard", Price = 1499.00m, Quantity = 25, CreatedAt = new DateTime(2026, 1, 1) },
                    new Product { Id = 2, Name = "Mouse", Price = 599.00m, Quantity = 60, CreatedAt = new DateTime(2026, 1, 1) },
                    new Product { Id = 3, Name = "Monitor", Price = 11999.00m, Quantity = 10, CreatedAt = new DateTime(2026, 1, 1) }
            );
        }


    }
}

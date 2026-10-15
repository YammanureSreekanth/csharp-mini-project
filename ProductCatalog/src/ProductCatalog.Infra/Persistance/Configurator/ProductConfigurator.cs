using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.App.Models.Entities;

namespace ProductCatalog.Infra.Persistance.Configurator;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name).IsRequired().HasMaxLength(100);

        builder.Property(l => l.Description);

        builder.Property(l => l.Online).HasDefaultValue(false);

        builder.Property(l => l.Searchable).HasDefaultValue(false);

        builder.Property(l => l.CategoryId);

        builder.Property(l => l.Price);

        builder.Property(l => l.Ats);
    }
}

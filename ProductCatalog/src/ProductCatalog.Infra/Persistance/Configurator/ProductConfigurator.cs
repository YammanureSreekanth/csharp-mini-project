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

        builder.HasIndex(l => l.CategoryId);

        builder
            .HasMany(l => l.Variants)
            .WithOne(l => l.BaseProduct)
            .HasForeignKey(l => l.BaseProductId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.App.Models.Entities;

namespace ProductCatalog.Infra.Persistance.Configurator;

public class VariantConfiguration : IEntityTypeConfiguration<Variant>
{
    public void Configure(EntityTypeBuilder<Variant> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.AttributeId);

        builder.Property(l => l.AttributeValue);

        builder.Property(l => l.Online).HasDefaultValue(false);

        builder.Property(l => l.Searchable).HasDefaultValue(false);
    }
}

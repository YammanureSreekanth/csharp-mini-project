using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.App.Models.Entities;

namespace ProductCatalog.Infra.Persistance.Configurator;

public class SellableSkuConfiguration : IEntityTypeConfiguration<SellableSku>
{
    public void Configure(EntityTypeBuilder<SellableSku> builder)
    {
        builder.HasKey(l => l.Id);
    }
}

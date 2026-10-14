using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.App.Models.Entities;

namespace ProductCatalog.Infra.Persistance.Configurator;

public class SkuInventoryConfigurator : IEntityTypeConfiguration<SkuInventory>
{
    public void Configure(EntityTypeBuilder<SkuInventory> builder)
    {
        builder.HasKey(l => l.SkuId);

        builder.HasOne<SellableSku>().WithOne().HasForeignKey<SkuInventory>(l => l.SkuId);

        builder.Property(l => l.ATS);
    }
}

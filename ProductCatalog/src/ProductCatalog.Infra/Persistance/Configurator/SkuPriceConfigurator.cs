using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProductCatalog.App.Models.Entities;

namespace ProductCatalog.Infra.Persistance.Configurator;

public class SkuPriceConfigurator : IEntityTypeConfiguration<SkuPrice>
{
    public void Configure(EntityTypeBuilder<SkuPrice> builder)
    {
        ValueConverter<Money, decimal>? converter = new ValueConverter<Money, decimal>(
            l => l.Amount,
            l => new Money(l, Currency.EUR)
        );

        builder.HasKey(l => l.SkuId);

        builder.HasOne<SellableSku>().WithOne().HasForeignKey<SkuPrice>(l => l.SkuId);

        builder.Property(l => l.Amount).HasConversion(converter);
    }
}

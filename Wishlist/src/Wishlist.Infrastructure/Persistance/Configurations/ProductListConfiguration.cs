using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wishlist.Core.Entities;

namespace Wishlist.Infrastructure.Persistence.Configurations;

public class ProductListConfiguration : IEntityTypeConfiguration<ProductList>
{
    public void Configure(EntityTypeBuilder<ProductList> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.CustomerId)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(l => l.CustomerId);

        builder.HasMany(l => l.Items)
            .WithOne()
            .HasForeignKey(i => i.ProductListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
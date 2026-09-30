using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wishlist.Core.Entities;

namespace Wishlist.Infrastructure.Persistence.Configurations;

public class ProductListItemConfiguration : IEntityTypeConfiguration<ProductListItem>
{
    public void Configure(EntityTypeBuilder<ProductListItem> builder)
    {
       builder.HasKey(k => k.Id);
       
       builder.Property(k => k.ProductId)
            .IsRequired()
            .HasMaxLength(50);
       
       builder.Property(k => k.Quantity)
            .HasDefaultValue((short)1);
        
       builder.HasIndex(k => k.ProductListId);

    }
}
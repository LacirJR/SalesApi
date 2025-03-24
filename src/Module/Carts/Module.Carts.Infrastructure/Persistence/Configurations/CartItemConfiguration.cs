using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Module.Carts.Domain.Entities;

namespace Module.Carts.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.Ignore(b => b.DomainEvents);

        builder.HasKey(b => b.Id);

        builder.HasIndex(b => b.CartId);
        builder.HasIndex(b => b.ProductId);

        builder.Property(b => b.CartId).IsRequired();
        builder.Property(b => b.ProductId).IsRequired();
        builder.Property(b => b.Quantity).IsRequired().HasDefaultValue(1);
        builder.Property(b => b.DiscountPercentage).HasPrecision(5, 2).HasDefaultValue(0);
        builder.Property(b => b.UnitPrice).HasPrecision(18, 2) .IsRequired();
        
        builder.HasOne(ci => ci.Cart)
            .WithMany(c => c.Products)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
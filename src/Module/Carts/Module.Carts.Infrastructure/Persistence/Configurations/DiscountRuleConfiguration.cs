using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Module.Carts.Domain.Entities;

namespace Module.Carts.Infrastructure.Persistence.Configurations;

public class DiscountRuleConfiguration :  IEntityTypeConfiguration<DiscountRule>
{
    public void Configure(EntityTypeBuilder<DiscountRule> builder)
    {
        builder.Ignore(b => b.DomainEvents);
        
        builder.HasKey(b => b.Id);

        builder.Property(b => b.MinQuantity).IsRequired();
        builder.Property(b => b.MaxQuantity).IsRequired();
        builder.Property(b => b.DiscountPercentage).HasPrecision(5, 2).IsRequired();
        builder.Property(b => b.Active).IsRequired().HasDefaultValue(true);
    }
}
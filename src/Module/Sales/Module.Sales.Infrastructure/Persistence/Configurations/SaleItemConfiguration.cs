using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Module.Sales.Domain.Entities;

namespace Module.Sales.Infrastructure.Persistence.Configurations;

public class SaleItemConfiguration :  IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.Ignore(x => x.DomainEvents);

        builder.HasKey(i => i.Id);

        builder.Property(i => i.SaleId)
            .IsRequired();

        builder.Property(i => i.ProductId)
            .IsRequired();

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.UnitPrice)
            .IsRequired()
            .HasPrecision(18,2);

        builder.Property(i => i.DiscountPercentage)
            .IsRequired()
            .HasPrecision(5,2);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Module.Sales.Domain.Entities;

namespace Module.Sales.Infrastructure.Persistence.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        
        builder.Ignore(x => x.DomainEvents);
        
        
        builder.HasKey(s => s.Id);

        builder.HasIndex(x => x.Number).IsUnique();

        builder.Property(s => s.Number)
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder.Property(s => s.Date)
            .IsRequired();

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.Branch)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.CartId)
            .IsRequired();
        

        builder.HasMany(s => s.Items)
            .WithOne(i => i.Sale)
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
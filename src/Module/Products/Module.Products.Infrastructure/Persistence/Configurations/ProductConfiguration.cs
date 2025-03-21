using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Module.Products.Domain.Entities;

namespace Module.Products.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Ignore(e => e.DomainEvents);
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasPrecision(18, 2); 

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Image)
            .HasMaxLength(500);

        builder.Property(p => p.CategoryId)
            .IsRequired();

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(p => p.Rating, rating =>
        {
            rating.Property(r => r.Rate)
                .HasColumnName("RatingRate")
                .HasPrecision(3, 2);

            rating.Property(r => r.Count)
                .HasColumnName("RatingCount");
        });
        
    }
}
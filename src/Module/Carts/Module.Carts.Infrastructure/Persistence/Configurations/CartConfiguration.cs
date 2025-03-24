using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Module.Carts.Domain.Entities;

namespace Module.Carts.Infrastructure.Persistence.Configurations;

public class CartConfiguration :  IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.Ignore(x => x.DomainEvents);
        
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(c => c.UserId);
        
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Date).IsRequired();
        
        builder.HasMany(c => c.Products)
            .WithOne(x => x.Cart)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade); 

        builder.Navigation(c => c.Products).AutoInclude();

        
    }
}
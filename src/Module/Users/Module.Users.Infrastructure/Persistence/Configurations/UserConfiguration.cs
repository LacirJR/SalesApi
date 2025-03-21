using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Module.Users.Domain.Entities;

namespace Module.Users.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Ignore(e => e.DomainEvents);
        
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.Email).IsUnique();
        
        
        builder.OwnsOne(x => x.Name, name =>
        {
            name.Property(n => n.Firstname).HasColumnName("Firstname").HasMaxLength(50);
            name.Property(n => n.Lastname).HasColumnName("Lastname").HasMaxLength(50);
        });
        
        builder.OwnsOne(u => u.Address, address =>
        {
            address.Property(a => a.City).HasColumnName("City").HasMaxLength(100);
            address.Property(a => a.Street).HasColumnName("Street").HasMaxLength(100);
            address.Property(a => a.Zipcode).HasColumnName("Zipcode").HasMaxLength(20);
            address.Property(a => a.Number).HasColumnName("Number").HasMaxLength(20);

            address.OwnsOne(a => a.Geolocation, geo =>
            {
                geo.Property(g => g.Lat).HasColumnName("Lat").HasMaxLength(20);
                geo.Property(g => g.Long).HasColumnName("Long").HasMaxLength(20);
            });
        });
        
        builder.Property(u => u.Email).HasMaxLength(100);
        builder.Property(u => u.Username).HasMaxLength(50);
        builder.Property(u => u.Password).HasMaxLength(255);
        builder.Property(u => u.Phone).HasMaxLength(20);
        builder.Property(u => u.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
    }
}
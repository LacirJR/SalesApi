using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.Carts.Application.Interfaces.Persistence;
using Module.Carts.Infrastructure.Persistence;
using Module.Carts.Infrastructure.Persistence.Repositories;
using Module.Carts.Infrastructure.Persistence.Seeders;
using Shared.Infrastructure.Extensions;

namespace Module.Carts.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCartInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddScoped<ICartDbContext>(provider => provider.GetService<CartDbContext>())
            .AddDatabaseContext<CartDbContext>(config)
            .AddScoped<CartDbContextInitializer>();

        services.AddScoped<IDiscountRuleRepository, DiscountRuleRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        
        return services;
    }
}
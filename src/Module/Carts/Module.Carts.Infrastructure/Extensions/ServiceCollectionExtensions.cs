using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.Carts.Application.Interfaces.Persistence;
using Module.Carts.Infrastructure.Consumers;
using Module.Carts.Infrastructure.Persistence;
using Module.Carts.Infrastructure.Persistence.Repositories;
using Module.Carts.Infrastructure.Persistence.Seeders;
using Shared.Application.Interfaces.Persistence;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Persistence;

namespace Module.Carts.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCartInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddScoped<ICartDbContext>(provider => provider.GetService<CartDbContext>())
            .AddDatabaseContext<CartDbContext>(config)
            .AddScoped<CartDbContextInitializer>();

        services.AddScoped<ICartUnitOfWork, CartUnitOfWork>();

        
        services.AddScoped<IDiscountRuleRepository, DiscountRuleRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ISharedCartRepository, SharedCartRepository>();         

        
        MassTransitConfiguratorBus.MassTransitConfigurator.AddConsumer<ProductDeletedCartConsumer>();
        
        return services;
    }
}
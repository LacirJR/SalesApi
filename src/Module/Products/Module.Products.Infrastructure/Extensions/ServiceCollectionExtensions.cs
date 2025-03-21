using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Infrastructure.Persistence;
using Module.Products.Infrastructure.Persistence.Repositories;
using Shared.Application.Interfaces.Persistence;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Persistence;

namespace Module.Products.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProductsInfrastructure(this IServiceCollection services, IConfiguration config)
    {

        services
            .AddScoped<IProductDbContext>(provider => provider.GetService<ProductDbContext>())
            .AddDatabaseContext<ProductDbContext>(config);
        
        services.AddScoped<IProductRepository, ProductRepository>();         
        services.AddScoped<ICategoryRepository, CategoryRepository>();         
        return services;
    }
}
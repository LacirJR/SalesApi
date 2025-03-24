using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.Sales.Application.Interfaces.Persistence;
using Module.Sales.Infrastructure.Consumers;
using Module.Sales.Infrastructure.Persistence;
using Module.Sales.Infrastructure.Persistence.Repositories;
using Shared.Application.Interfaces.Persistence;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Persistence;

namespace Module.Sales.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSalesInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddScoped<ISaleDbContext>(provider => provider.GetService<SaleDbContext>())
            .AddDatabaseContext<SaleDbContext>(config);

        services.AddScoped<ISaleUnitOfWork, SaleUnitOfWork>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<ISharedSaleRepository, SharedSaleRepository>();

        MassTransitConfiguratorBus.MassTransitConfigurator.AddConsumer<ProductDeletedSaleConsumer>();

        return services;
    }
}
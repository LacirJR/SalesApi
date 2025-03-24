using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.Sales.Application.Extensions;
using Module.Sales.Infrastructure.Extensions;

namespace Module.Sales.Presentation.Extensions;

public static class ModuleExtensionsCollector
{
    public static IServiceCollection AddSaleModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSalesApplication(configuration)
            .AddSalesInfrastructure(configuration);

        return services;
    }
}
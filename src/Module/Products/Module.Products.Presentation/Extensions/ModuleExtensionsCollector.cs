using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.Products.Application.Extensions;
using Module.Products.Infrastructure.Extensions;

namespace Module.Products.Presentation.Extensions;

public static class ModuleExtensionsCollector
{
    public static IServiceCollection AddProductModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddProductApplication(configuration)
            .AddProductsInfrastructure(configuration);

        return services;
    }
}
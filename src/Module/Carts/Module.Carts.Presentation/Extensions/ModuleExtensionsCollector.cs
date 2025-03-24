using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.Carts.Application.Extensions;
using Module.Carts.Infrastructure.Extensions;

namespace Module.Carts.Presentation.Extensions;

public static class ModuleExtensionsCollector
{
    public static IServiceCollection AddCartModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCartApplication(configuration)
            .AddCartInfrastructure(configuration);

        return services;
    }
}
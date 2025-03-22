using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Services;
using Shared.Application.Services;

namespace Shared.Application.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSharedApplication(this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddScoped<ISharedProductService, SharedProductService>();
        services.AddScoped<ISharedUserService, SharedUserService>();
        
        return services;
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Interfaces.Persistence;

namespace Shared.Application.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSharedApplication(this IServiceCollection services,
        IConfiguration configuration)
    {
        
        return services;
    }
}
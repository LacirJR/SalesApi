using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Module.Users.Application.Extensions;
using Module.Users.Infrastructure.Extensions;


namespace Module.Users.Extensions;

public static class ModuleExtensionsCollector
{
    public static IServiceCollection AddUserModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services
            .AddUsersApplication(configuration)
            .AddUsersInfrastructure(configuration);

        return services;
    }
}
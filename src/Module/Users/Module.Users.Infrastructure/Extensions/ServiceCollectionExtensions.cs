using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.Users.Application.Interfaces.Persistence;
using Module.Users.Infrastructure.Persistence;
using Module.Users.Infrastructure.Persistence.Repositories;
using Module.Users.Infrastructure.Persistence.Seeders;
using Shared.Application.Interfaces.Persistence;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Persistence;

namespace Module.Users.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services, IConfiguration config)
    {

        services
            .AddScoped<IUserDbContext>(provider => provider.GetService<UserDbContext>())
            .AddDatabaseContext<UserDbContext>(config)
            .AddScoped<UserDbContextInitializer>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork<UserDbContext>>();


        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }

}
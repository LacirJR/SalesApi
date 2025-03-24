using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.Users.Application.Interfaces;
using Module.Users.Application.Interfaces.Authentication;
using Module.Users.Application.Interfaces.Persistence;
using Module.Users.Infrastructure.Authentication;
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
        
        services.AddScoped<IUserUnitOfWork, UserUnitOfWork>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISharedUserRepository, SharedUserRepository>();
        
        return services;
    }

}
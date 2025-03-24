using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Common;
using Shared.Application.Interfaces;
using Shared.Application.Interfaces.Persistence;
using Shared.Infrastructure.Persistence;

namespace Shared.Infrastructure.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddMassTransitWithRabbitMq(configuration);
        return services;
    }
    
    //Use to inject dbContext for any module 
    public static IServiceCollection AddDatabaseContext<T>(this IServiceCollection services,
        IConfiguration configuration)
        where T : DbContext
    {
        string defaultConnectionString = configuration.GetConnectionString("DefaultConnection")!;
        services.AddNpgSql<T>(defaultConnectionString);

        return services;
    }
    
    public static IServiceCollection AddNpgSql<T>(this IServiceCollection services, string connectionString)
        where T : DbContext
    {
        services.AddDbContext<T>(options =>
            options.UseNpgsql(connectionString, o =>
            {
                o.MigrationsAssembly(typeof(T).Assembly.FullName);
                o.UseNetTopologySuite();
            }));

        using var scope = services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<T>();
        dbContext.Database.Migrate();
        return services;
    }

}
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Module.Carts.Presentation.Extensions;
using Module.Products.Presentation.Extensions;
using Module.Users.Extensions;
using Module.Users.Infrastructure.Persistence.Seeders;
using Shared.Infrastructure.Extensions;
using Shared.Presentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "SalesAPI", Version = "v1",
        Description =
            "API for managing users, authentication, products, carts, and sales. Built with .NET 8, featuring modular architecture, Domain-Driven Design (DDD), Mediator & CQRS patterns, standardized pagination, filtering, ordering, and consistent error handling."
    });

    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            @$"Send 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
    
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    c.IncludeXmlComments(xmlPath, true);
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddSharedInfrastructure(builder.Configuration);

builder.Services.AddUserModule(builder.Configuration)
    .AddProductModule(builder.Configuration)
    .AddCartModule(builder.Configuration);

builder.Services.AddHealthChecks();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    //Seed database
    using (var scope = app.Services.CreateScope())
    {
        var initialiserUserDbContextInitializer = scope.ServiceProvider.GetRequiredService<UserDbContextInitializer>();
        await initialiserUserDbContextInitializer.SeedAsync();
    }
}

//app.UseHttpsRedirection();
app.MapHealthChecks("/api/health");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();
app.Run();
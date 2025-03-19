using Api.Middlewares;
using Module.Users.Extensions;
using Module.Users.Infrastructure.Persistence.Seeders;
using Shared.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddUserModule(builder.Configuration);

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

app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();
app.Run();
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Domain.Entities;
using Shared.Application.Interfaces;
using Shared.Infrastructure.Persistence;

namespace Module.Products.Infrastructure.Persistence;

public class ProductDbContext : ModuleDbContext, IProductDbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProductDbContext(DbContextOptions<ProductDbContext> options, IMediator mediator,
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor) : base(options, mediator, currentUserService, httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override string Schema => "Products";

    public DbSet<Product> Products { get; set; }
    public DbSet<Category?> Categories { get; set; }
}
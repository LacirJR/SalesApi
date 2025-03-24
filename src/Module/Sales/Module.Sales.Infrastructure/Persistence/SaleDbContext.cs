using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Module.Sales.Application.Interfaces.Persistence;
using Module.Sales.Domain.Entities;
using Shared.Application.Interfaces;
using Shared.Infrastructure.Persistence;

namespace Module.Sales.Infrastructure.Persistence;

public class SaleDbContext : ModuleDbContext, ISaleDbContext
{

    public SaleDbContext(DbContextOptions<SaleDbContext> options, IMediator mediator,
        ICurrentUserService currentUserService) : base(options, mediator, currentUserService)
    {
    }

    protected override string Schema => "Sales";
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }
    
}
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shared.Application.Interfaces;
using Shared.Domain.Common;
using Shared.Infrastructure.Common;

namespace Shared.Infrastructure.Persistence;

public abstract class ModuleDbContext : DbContext
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;


    protected ModuleDbContext(DbContextOptions options, IMediator mediator, ICurrentUserService currentUserService) : base(options)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    protected abstract string Schema { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (!string.IsNullOrWhiteSpace(Schema))
        {
            modelBuilder.HasDefaultSchema(Schema);
        }

        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker.Entries<BaseEntity>();
        foreach (var entity in entities)
        {
            if (entity.State == EntityState.Added)
            {
                entity.Entity.UpdatedAtUtc = DateTime.UtcNow;
                entity.Entity.CreatedAtUtc = DateTime.UtcNow;
                entity.Entity.CreatedBy = _currentUserService.UserId.ToString();
            }

            if (entity.State == EntityState.Modified || entity.HasChangedOwnedEntities())
            {
                entity.Entity.CreatedAtUtc = entity.Entity.CreatedAtUtc;
                entity.Entity.UpdatedAtUtc = DateTime.UtcNow;
                entity.Entity.ModifiedBy = _currentUserService.UserId.ToString();
                entity.Entity.CreatedBy = entity.Entity.CreatedBy;
            }
        }

        await _mediator.DispatchDomainEvents(this);
        
        return (await base.SaveChangesAsync(true, cancellationToken));
    }
    
}
public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
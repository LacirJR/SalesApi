using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Module.Users.Application.Interfaces.Persistence;
using Module.Users.Domain.Entities;
using Shared.Application.Interfaces;
using Shared.Infrastructure.Persistence;

namespace Module.Users.Infrastructure.Persistence;

public class UserDbContext : ModuleDbContext, IUserDbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options, IMediator mediator,
        ICurrentUserService currentUserService) : base(options, mediator, currentUserService)
    {
    }

    protected override string Schema => "Users";
    public DbSet<User> Users { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
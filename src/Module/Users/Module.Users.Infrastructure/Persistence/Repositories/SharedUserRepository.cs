using Microsoft.EntityFrameworkCore;
using Module.Users.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Persistence;

namespace Module.Users.Infrastructure.Persistence.Repositories;

public class SharedUserRepository : ISharedUserRepository
{
    private readonly IUserDbContext _context;

    public SharedUserRepository(IUserDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Users.AnyAsync(u => u.Id == userId, cancellationToken);
    }
}
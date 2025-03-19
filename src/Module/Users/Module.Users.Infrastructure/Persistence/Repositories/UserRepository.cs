using Microsoft.EntityFrameworkCore;
using Module.Users.Application.Interfaces.Persistence;
using Module.Users.Domain.Entities;

namespace Module.Users.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private IUserDbContext _context;

    public UserRepository(IUserDbContext context)
    {
        _context = context;
    }


    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}
using Gridify;
using Gridify.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Module.Users.Application.Interfaces.Persistence;
using Module.Users.Domain.Entities;
using Shared.Infrastructure.Common;

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
    
    public void Update(User user)
    {
        var existingUser = _context.Users.Local.FirstOrDefault(u => u.Id == user.Id);
    
        if (existingUser is null)
        {
            _context.Users.Attach(user);
        }
    
        _context.Users.Update(user);
    }
    
    public async Task<User?> RemoveByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var existingUser = await _context.Users.FindAsync(id, cancellationToken);
    
        if (existingUser is null) return null;

        _context.Users.Remove(existingUser);
        
        return existingUser;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
    
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Users.FindAsync(id, cancellationToken);
    }

    public async Task<PaginatedList<User>> GetAllAsync(string? filter, string? orderBy, int page, int size,
        CancellationToken cancellationToken)
    {
        var gridifyQuery = new GridifyQuery()
        {
            Filter = filter,
            OrderBy = orderBy,
            Page = page,
            PageSize = size,
        };
        var pagingUsers =  await _context.Users.AsNoTracking().GridifyAsync(gridifyQuery, cancellationToken);
        
        return new PaginatedList<User>(pagingUsers.Data, pagingUsers.Count, page, size);
        
        
    }
}
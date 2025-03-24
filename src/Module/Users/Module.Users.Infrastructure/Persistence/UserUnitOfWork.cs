using Module.Users.Application.Interfaces.Persistence;
using Shared.Infrastructure.Persistence;

namespace Module.Users.Infrastructure.Persistence;

public class UserUnitOfWork : UnitOfWork<UserDbContext>, IUserUnitOfWork
{
    public UserUnitOfWork(UserDbContext context) : base(context) { }
}
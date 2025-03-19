namespace Shared.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
}
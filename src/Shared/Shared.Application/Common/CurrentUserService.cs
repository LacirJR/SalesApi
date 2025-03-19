using Microsoft.AspNetCore.Http;
using Shared.Application.Interfaces;

namespace Shared.Application.Common;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId => GetUserId();

    public Guid? GetUserId()
    {
        
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || user.Identity?.IsAuthenticated == false)
        {
            return null;
        }
        
        var claim = _httpContextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == "UserId");

        return claim != null
            ? Guid.Parse(claim.Value)
            : throw new UnauthorizedAccessException("User ID not found in claims.");
        ;
    }
}
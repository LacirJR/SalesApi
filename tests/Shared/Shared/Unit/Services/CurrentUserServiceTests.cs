using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Shared.Application.Common;
using Shared.Application.Interfaces;
using Xunit;

namespace Shared.Unit.Services;

public class CurrentUserServiceTests
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICurrentUserService _currentUserService;
    
    public CurrentUserServiceTests()
    {
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _currentUserService = new CurrentUserService(_httpContextAccessor);
    }
    
    [Fact]
    public void GetUserId_Should_Return_Null_When_HttpContext_Is_Null()
    {
        _httpContextAccessor.HttpContext.Returns((HttpContext)null);

        var userId = _currentUserService.UserId;

        Assert.Null(userId);
    }
    
    [Fact]
    public void GetUserId_Should_Return_Null_When_User_Is_Not_Authenticated()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity());
        _httpContextAccessor.HttpContext.Returns(context);

        var userId = _currentUserService.UserId;

        Assert.Null(userId);
    }
    
}
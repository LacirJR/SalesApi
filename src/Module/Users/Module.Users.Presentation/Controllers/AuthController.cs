using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Module.Users.Application.Commands.Auth.LoginCommand;
using Module.Users.Application.Interfaces.Persistence;
using Shared.Application.Exceptions;

namespace Module.Users.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _sender.Send(command);
        
        if(result.Succeeded)
            return Ok(result);

        throw new ServiceResultException(result.Error);
    }
}
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Module.Users.Application.Commands.CreateUserCommand;
using Shared.Application.Interfaces;
using Shared.Domain.Common;

namespace Module.Users.Controllers;
//TODO: Colocar autorize
//[Authorize]
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly ISender _sender;

    public UserController(ISender sender)
    {
        _sender = sender;
    }

 /// <summary>
 /// Add a new user
 /// </summary>
 /// <param name="command">Dados novo usuário</param>
 /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        var response = await _sender.Send(command);
        return Ok(response);
    }
}
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Module.Users.Application.Commands.CreateUserCommand;
using Module.Users.Application.Commands.DeleteUserCommand;
using Module.Users.Application.Commands.UpdateUserCommand;
using Module.Users.Application.Dtos;
using Module.Users.Application.Queries.GetUserByIdQuery;
using Module.Users.Application.Queries.GetUsersQuery;
using Shared.Application.Exceptions;
using Shared.Application.Interfaces;
using Shared.Domain.Common;
using Shared.Infrastructure.Common;

namespace Module.Users.Controllers;

[Authorize]
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
    /// Get users with pagination
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<PaginatedList<UserResponseDto>>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetUsersQuery query)
    {
        var response = await _sender.Send(query);
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }

    /// <summary>
    /// Get user by id
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<UserResponseDto>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _sender.Send(new GetUserByIdQuery(id));
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }


    /// <summary>
    /// Add a new user
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<UserResponseDto>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        var response = await _sender.Send(command);
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }

    /// <summary>
    /// Update User
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<UserResponseDto>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto updateUserDto)
    {
        var response = await _sender.Send(new UpdateUserCommand(id,
            updateUserDto.Email, updateUserDto.Username, updateUserDto.Password, updateUserDto.Name,
            updateUserDto.Address, updateUserDto.Phone, updateUserDto.Status, updateUserDto.Role));

        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }

    /// <summary>
    /// Delete User
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<UserResponseDto>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Update(Guid id)
    {
        var response = await _sender.Send(new DeleteUserCommand(id));

        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
        ;
    }
}
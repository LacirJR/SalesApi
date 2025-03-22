using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Module.Carts.Application.Commands.CreateCartCommand;
using Module.Carts.Application.Commands.DeleteCartCommand;
using Module.Carts.Application.Commands.UpdateCartCommand;
using Module.Carts.Application.Dtos;
using Module.Carts.Application.Queries.GetAllCartsQuery;
using Module.Carts.Application.Queries.GetCartByIdQuery;
using Shared.Application.Exceptions;
using Shared.Domain.Common;
using Shared.Infrastructure.Common;

namespace Module.Carts.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/carts")]
public class CartController : ControllerBase
{
    private readonly ISender _sender;

    public CartController(ISender sender)
    {
        _sender = sender;
    }
    
    
    /// <summary>
    ///  Retrieve a list of all carts
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<PaginatedList<CartResponseDto>>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpGet]
    public async Task<IActionResult> GetAll(string filter, string order, int page = 1, int size = 10)
    {
        var response = await _sender.Send(new GetAllCartsQuery(filter, order, page, size));
               
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }
    
    
    
    /// <summary>
    ///  Add a new cart
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<CartResponseDto>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCartCommand command)
    {
        var response = await _sender.Send(command);
              
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }

    /// <summary>
    ///  Retrieve a specific cart by ID
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<CartResponseDto>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _sender.Send(new GetCartByIdQuery(id));
               
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }
    
    /// <summary>
    ///  Update a specific cart
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<CartResponseDto>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCartDto dto)
    {
        var response = await _sender.Send(new UpdateCartCommand(id, dto.UserId, dto.Date, dto.Products));
               
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }
    
    /// <summary>
    ///  Delete a specific cart
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<CartResponseDto>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _sender.Send(new DeleteCartCommand(id));
               
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }
    
}
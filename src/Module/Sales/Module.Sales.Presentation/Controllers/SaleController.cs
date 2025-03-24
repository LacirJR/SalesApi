using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Module.Sales.Application.Commands.CancelSaleCommand;
using Module.Sales.Application.Commands.CreateFromCartCommand;
using Module.Sales.Application.Commands.FinishSaleCommand;
using Module.Sales.Application.Commands.RemoveSaleItemCommand;
using Module.Sales.Application.Commands.UpdateSaleCommand;
using Module.Sales.Application.Dtos;
using Module.Sales.Application.Queries.GetAllSalesQuery;
using Module.Sales.Application.Queries.GetSaleByIdQuery;
using Module.Sales.Application.Queries.GetSaleByNumberQuery;
using Shared.Application.Exceptions;
using Shared.Domain.Common;
using Shared.Infrastructure.Common;

namespace Module.Sales.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/sales")]
public class SaleController : ControllerBase
{
    private readonly ISender _sender;

    public SaleController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get sales with pagination
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<PaginatedList<SaleResponseDto>>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpGet]
    public async Task<IActionResult> GetAll(string? filter, string? order, int page, int size)
    {
        var response = await _sender.Send(new GetAllSalesQuery(filter, order, page, size));
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }
    
    /// <summary>
    /// Get sale by id
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<SaleResponseDto>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _sender.Send(new GetSaleByIdQuery(id));
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }
    
    /// <summary>
    /// Get sale by number
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<SaleResponseDto>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpGet("by-number/{number}")]
    public async Task<IActionResult> GetByNumber(long number)
    {
        var response = await _sender.Send(new GetSaleByNumberQuery(number));
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }
    
    /// <summary>
    /// Create Sale with cart id
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<SaleResponseDto>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSaleFromCartCommand command)
    {
        var response = await _sender.Send(command);
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }
    
    /// <summary>
    /// Update Sale by id
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<SaleResponseDto>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSaleDto dto)
    {
        var response = await _sender.Send(new UpdateSaleCommand(id, dto.Branch, dto.Date));
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }
    
    /// <summary>
    /// Change Status of sale for finalized
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<SaleResponseDto>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpPut("{id}/finish")]
    public async Task<IActionResult> Finish(Guid id)
    {
        var response = await _sender.Send(new FinishSaleCommand(id));
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }
    
    /// <summary>
    /// Cancel sale
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<SaleResponseDto>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpDelete("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var response = await _sender.Send(new CancelSaleCommand(id));
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }
    
    /// <summary>
    /// Remove item salve by product Id
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<SaleResponseDto>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpDelete("{id}/remove/{productId}/item")]
    public async Task<IActionResult> CancelItem(Guid id, Guid productId)
    {
        var response = await _sender.Send(new RemoveSaleItemCommand(id, productId));
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);    }
}
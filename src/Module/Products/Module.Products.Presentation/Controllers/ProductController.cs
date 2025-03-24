using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Module.Products.Application.Commands.CreateProductCommand;
using Module.Products.Application.Commands.DeleteProductCommand;
using Module.Products.Application.Commands.UpdateProductCommand;
using Module.Products.Application.Dtos;
using Module.Products.Application.Queries.GetCategoriesQuery;
using Module.Products.Application.Queries.GetProductByIdQuery;
using Module.Products.Application.Queries.GetProductsByCategoryNameQuery;
using Module.Products.Application.Queries.GetProductsQuery;
using Shared.Application.Exceptions;
using Shared.Domain.Common;
using Shared.Infrastructure.Common;

namespace Module.Products.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly ISender _sender;

    public ProductController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    ///  Retrieve a list of all products
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<PaginatedList<ProductResponseDto>>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] GetAllProductsQuery query)
    {
        var response = await _sender.Send(query);
        
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }

    /// <summary>
    ///  Add a new product
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<ProductResponseDto>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var response = await _sender.Send(command);
        
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }

    /// <summary>
    ///  Retrieve a specific product by ID
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<ProductResponseDto>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdProducts(Guid id)
    {
        var response = await _sender.Send(new GetProductByIdQuery(id));
        
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }

    /// <summary>
    ///  Update a specific product
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<ProductResponseDto>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        var response = await _sender.Send(new UpdateProductCommand(id, dto.Title, dto.Price, dto.Description,
            dto.Category, dto.Image, dto.Rating));
        
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }

    /// <summary>
    ///  Delete a specific product
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<ProductResponseDto>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _sender.Send(new DeleteProductCommand(id));
        
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
        ;
    }

    /// <summary>
    ///  Retrieve all product categories
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<List<string>>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var response = await _sender.Send(new GetCategoriesQuery());
        
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }

    /// <summary>
    ///  Retrieve products in a specific category
    /// </summary>
    [ProducesResponseType(typeof(ServiceResult<PaginatedList<ProductResponseDto>>), 200)]
    [ProducesResponseType(typeof(ServiceError), 400)]
    [ProducesResponseType(typeof(ServiceError), 500)]
    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetProductsByCategory(string category, string? order, int page = 1, int size = 10)
    {
        var response = await _sender.Send(new GetProductsByCategoryNameQuery(category, order, page, size));
        
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }
}
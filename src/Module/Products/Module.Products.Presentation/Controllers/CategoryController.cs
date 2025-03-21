using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Module.Products.Application.Commands.CreateCategoryCommand;
using Module.Products.Application.Commands.DeleteCategoryCommand;
using Shared.Application.Exceptions;

namespace Module.Products.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ISender _sender;

    public CategoryController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
    {
        var response = await _sender.Send(command);
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }
    
    [HttpDelete("{categoryName}")]
    public async Task<IActionResult> Delete(string categoryName)
    {
        var response = await _sender.Send(new DeleteCategoryCommand(categoryName));
        if (response.Succeeded)
            return Ok(response);

        throw new ServiceResultException(response.Error);
    }
}
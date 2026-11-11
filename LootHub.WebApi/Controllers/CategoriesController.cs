using LootHub.Application.Common;
using LootHub.Application.Contracts.Services;
using LootHub.Application.DTOs.Category;
using Microsoft.AspNetCore.Mvc;

namespace LootHub.WebApi.Controllers;

[Route("api/categories")]
public class CategoriesController(ICategoryService categoryService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var categories = await categoryService.GetAllAsync(ct);

        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await categoryService.GetByIdAsync(id, ct);

        return result.IsSuccess
            ? Ok(result.Data)
            : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken ct)
    {
        var result = await categoryService.CreateAsync(request, ct);

        return result.IsSuccess
           ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
           : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken ct)
    {
        var result = await categoryService.UpdateAsync(id, request, ct);

        return result.IsSuccess
            ? NoContent()
            : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await categoryService.DeleteAsync(id, ct);

        return result.IsSuccess
            ? NoContent()
            : Failure(result.ErrorType, result.ErrorMessage);
    }
}

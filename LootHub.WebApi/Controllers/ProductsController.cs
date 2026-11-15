using LootHub.Application.Common;
using LootHub.Application.Contracts.Services;
using LootHub.Application.DTOs.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LootHub.WebApi.Controllers;

[Route("api/products")]
public class ProductsController(IProductService service) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] ProductQuery query, CancellationToken ct)
    {
        var result = await service.GetPagedAsync(query, ct);
        return Ok(result);
    }


    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
    {
        var result = await service.GetBySlugAsync(slug, ct);

        return result.IsSuccess
           ? Ok(result.Data)
           : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        var result = await service.CreateAsync(request, ct);

        return result.IsSuccess
          ? CreatedAtAction(nameof(GetBySlug), new { slug = result.Data!.Slug }, result.Data)
          : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct)
    {
        var result = await service.UpdateAsync(id, request, ct);

        return result.IsSuccess
           ? NoContent()
           : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await service.DeleteAsync(id, ct);

        return result.IsSuccess
            ? NoContent()
            : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPatch("{id:guid}/stock")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateStockRequest request, CancellationToken ct)
    {
        var result = await service.UpdateStockAsync(id, request, ct);

        return result.IsSuccess
            ? NoContent()
            : Failure(result.ErrorType, result.ErrorMessage);
    }

}

using LootHub.Application.Contracts.Services;
using LootHub.Application.DTOs.Products;
using Microsoft.AspNetCore.Mvc;

namespace LootHub.WebApi.Controllers;

[Route("api/products")]
public class ProductsController(IProductService service) : ApiControllerBase
{

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
    {
        var result = await service.GetBySlugAsync(slug, ct);

        return result.IsSuccess
           ? Ok(result.Data)
           : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        var result = await service.CreateAsync(request, ct);

        return result.IsSuccess
          ? CreatedAtAction(nameof(GetBySlug), new { slug = result.Data!.Slug }, result.Data)
          : Failure(result.ErrorType, result.ErrorMessage);
    }

}

using LootHub.Application.Contracts.Services;
using LootHub.Application.DTOs.Brand;
using Microsoft.AspNetCore.Mvc;

namespace LootHub.WebApi.Controllers;

[Route("api/brands")]
public class BrandsController(IBrandService service) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var brands = await service.GetAllAsync(ct);

        return Ok(brands);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await service.GetByIdAsync(id, ct);

        return result.IsSuccess
            ? Ok(result.Data)
            : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBrandRequest request, CancellationToken ct)
    {
        var result = await service.CreateAsync(request, ct);

        return result.IsSuccess
           ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
           : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBrandRequest request , CancellationToken ct)
    {
        var result = await service.UpdateAsync(id ,request, ct);

        return result.IsSuccess
            ? NoContent()
            : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await service.DeleteAsync(id, ct);

        return result.IsSuccess
           ? NoContent()
           : Failure(result.ErrorType, result.ErrorMessage);
    }
}

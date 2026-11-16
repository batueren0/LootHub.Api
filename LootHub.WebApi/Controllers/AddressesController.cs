using LootHub.Application.Contracts.Services;
using LootHub.Application.DTOs.Address;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LootHub.WebApi.Controllers;

[Route("api/addresses")]
[Authorize]
public class AddressesController(IAddressService service) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var addresses = await service.GetAllAsync(CurrentUserId, ct);

        return Ok(addresses);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await service.GetByIdAsync(CurrentUserId, id, ct);

        return result.IsSuccess
            ? Ok(result.Data)
            : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAddressRequest request, CancellationToken ct)
    {
        var result = await service.CreateAsync(CurrentUserId, request, ct);

        return result.IsSuccess
           ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
           : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAddressRequest request, CancellationToken ct)
    {
        var result = await service.UpdateAsync(CurrentUserId, id, request, ct);

        return result.IsSuccess
           ? NoContent()
           : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await service.DeleteAsync(CurrentUserId, id, ct);

        return result.IsSuccess
            ? NoContent()
            : Failure(result.ErrorType, result.ErrorMessage);
    }
}

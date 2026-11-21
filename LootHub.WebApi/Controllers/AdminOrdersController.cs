using LootHub.Application.Common;
using LootHub.Application.Contracts.Services;
using LootHub.Application.DTOs.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LootHub.WebApi.Controllers;

[Route("api/admin/orders")]
[Authorize(Roles = Roles.Admin)]
public class AdminOrdersController(IOrderService service) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] AdminOrderQuery query, CancellationToken ct)
    {
        var result = await service.GetPagedForAdminAsync(query, ct);
        return Ok(result);
    }

    [HttpGet("{orderNumber}")]
    public async Task<IActionResult> GetByOrderNumber(string orderNumber, CancellationToken ct)
    {
        var result = await service.GetByOrderNumberForAdminAsync(orderNumber, ct);

        return result.IsSuccess
           ? Ok(result.Data)
           : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPatch("{orderNumber}/status")]
    public async Task<IActionResult> UpdateStatus(string orderNumber, [FromBody] UpdateOrderStatusRequest request, CancellationToken ct)
    {
        var result = await service.UpdateStatusAsync(orderNumber, request, ct);

        return result.IsSuccess
           ? NoContent()
           : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPost("{orderNumber}/cancel")]
    public async Task<IActionResult> Cancel(string orderNumber, CancellationToken ct)
    {
        var result = await service.CancelByAdminAsync(orderNumber, ct);

        return result.IsSuccess
           ? NoContent()
           : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPost("{orderNumber}/return")]
    public async Task<IActionResult> Return(string orderNumber, CancellationToken ct)
    {
        var result = await service.ReturnAsync(orderNumber, ct);

        return result.IsSuccess
           ? NoContent()
           : Failure(result.ErrorType, result.ErrorMessage);
    }
}

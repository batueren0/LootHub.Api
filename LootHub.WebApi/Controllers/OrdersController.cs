using LootHub.Application.Contracts.Services;
using LootHub.Application.DTOs.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LootHub.WebApi.Controllers;

[Route("api/orders")]
[Authorize]
public class OrdersController(IOrderService service) : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        var result = await service.CreateAsync(CurrentUserId, request, ct);

        return result.IsSuccess
          ? CreatedAtAction(nameof(GetByOrderNumber), new { orderNumber = result.Data!.OrderNumber }, result.Data)
          : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpGet]
    public async Task<IActionResult> GetMine(CancellationToken ct)
    {
        var result = await service.GetMyOrdersAsync(CurrentUserId, ct);

        return Ok(result);
    }

    [HttpGet("{orderNumber}")]
    public async Task<IActionResult> GetByOrderNumber(string orderNumber, CancellationToken ct)
    {
        var result = await service.GetByOrderNumberAsync(CurrentUserId, orderNumber, ct);

        return result.IsSuccess
           ? Ok(result.Data)
           : Failure(result.ErrorType, result.ErrorMessage);
    }


    [HttpPost("{orderNumber}/payments")]
    public async Task<IActionResult> StartPayment(string orderNumber, CancellationToken ct)
    {
        var result = await service.StartPaymentAsync(CurrentUserId, orderNumber, ct);

        return result.IsSuccess
           ? Ok(result.Data)
           : Failure(result.ErrorType, result.ErrorMessage);
    }
}
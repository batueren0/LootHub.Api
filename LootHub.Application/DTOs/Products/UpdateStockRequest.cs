using LootHub.Domain.Enums;

namespace LootHub.Application.DTOs.Products;

public sealed record UpdateStockRequest(
    StockAction Action,
    int Quantity
    );

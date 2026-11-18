using FluentValidation;
using LootHub.Application.Common;
using LootHub.Application.Contracts.Services;
using LootHub.Application.DTOs.Order;
using LootHub.Domain.Entities;
using LootHub.Domain.Enums;
using LootHub.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LootHub.Application.Features;

public class OrderService(IUnitOfWork uow,
    UserManager<AppUser> userManager,
    IValidator<CreateOrderRequest> createOrderValidator,
    IPaymentGateway paymentGateway
    ) : IOrderService
{
    public async Task<Result<OrderDetailResponse>> CreateAsync(Guid userId, CreateOrderRequest request, CancellationToken ct = default)
    {
        var validationResult = await createOrderValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Result<OrderDetailResponse>.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var address = await uow.UserAddresses.GetAsync(new QueryOptions<UserAddress>
        {
            Predicate = a => a.UserId == userId && a.Id == request.AddressId
        },ct);
        if (address is null)
            return Result<OrderDetailResponse>.Failure(ErrorType.NotFound, "Address not found.");

        var customer = await userManager.FindByIdAsync(userId.ToString());
        if (customer is null)
            return Result<OrderDetailResponse>.Failure(ErrorType.NotFound, "User not found.");

        var productsIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await uow.Products.GetListAsync(new QueryOptions<Product>
        {
            Predicate = p => productsIds.Contains(p.Id)
        }, ct);
        if (products.Count() != productsIds.Count)
            return Result<OrderDetailResponse>.Failure(ErrorType.NotFound, "One or more products were not found.");

        var productsById = products.ToDictionary(p => p.Id);

        foreach(var item in request.Items)
        {
            var product = productsById[item.ProductId];

            if (product.Stock < item.Quantity)
                return Result<OrderDetailResponse>.Failure(ErrorType.Conflict, $"Not enough stock for {product.Name}. Available: {product.Stock}.");
        }

        var items = request.Items.Select(i =>
        {
            var product = productsById[i.ProductId];
            return new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = Money.TRY(product.UnitPrice.Amount),
                Quantity = i.Quantity
            };
        }).ToList();

        var order = new Order
        {
            UserId = userId,
            OrderNumber = GenerateOrderNumber(),
            CustomerName = $"{customer.FirstName} {customer.LastName}",
            CustomerEmail = customer.Email!,
            Phone = customer.PhoneNumber ?? string.Empty,
            Address = Address.Create(address.Title, address.Details, address.City, address.Country),
            TotalAmount = Money.TRY(items.Sum(i => i.TotalAmount.Amount)),
            Status = OrderStatus.PendingPayment,
            Items = items
        };

        await uow.Orders.AddAsync(order, ct);
        await uow.SaveChangesAsync(ct);

        return Result<OrderDetailResponse>.Success(ToDetailResponse(order));
        
    }

    public async Task<Result<OrderDetailResponse>> GetByOrderNumberAsync(Guid userId, string orderNumber, CancellationToken ct = default)
    {
        var order = await uow.Orders.GetAsync(new QueryOptions<Order>
        {
            Predicate = o => o.UserId == userId && o.OrderNumber == orderNumber,
            Include = q => q.Include(o => o.Items)
        }, ct);
        if (order is null)
            return Result<OrderDetailResponse>.Failure(ErrorType.NotFound, "Order not found.");

        return Result<OrderDetailResponse>.Success(ToDetailResponse(order));
    }

    public async Task<IReadOnlyList<OrderSummaryResponse>> GetMyOrdersAsync(Guid userId, CancellationToken ct = default)
    {
        var orders = await uow.Orders.GetListAsync(new QueryOptions<Order>
        {
            Predicate = o => o.UserId == userId,
            OrderBy = o => o.OrderByDescending(o => o.CreatedAtUtc),
            Include = q => q.Include(o => o.Items)
        },ct);
        
        return orders.Select(ToSummaryResponse).ToList();
    }


    public async Task<Result<StartPaymentResponse>> StartPaymentAsync(Guid userId, string orderNumber, CancellationToken ct = default)
    {
        var order = await uow.Orders.GetAsync(new QueryOptions<Order>
        {
            Predicate = q => q.UserId == userId && q.OrderNumber == orderNumber,
            Include = q => q.Include(o => o.Items)
        }, ct);
        if (order is null)
            return Result<StartPaymentResponse>.Failure(ErrorType.NotFound, "Order not found.");

        if (order.Status != OrderStatus.PendingPayment)
            return Result<StartPaymentResponse>.Failure(ErrorType.Conflict, "Only orders awaiting payment can be paid");

        var result = await paymentGateway.InitializeAsync(order, ct);
        if (!result.IsSuccess)
            return Result<StartPaymentResponse>.Failure(ErrorType.Conflict, result.ErrorMessage ?? "Payment could not be started.");

        var payment = new Payment
        {
            OrderId = order.Id,
            Amount = Money.TRY(order.TotalAmount.Amount),
            Status = PaymentStatus.Pending,
            ProviderToken = result.Token
        };

        await uow.Payments.AddAsync(payment, ct);
        await uow.SaveChangesAsync(ct);

        return Result<StartPaymentResponse>.Success(new StartPaymentResponse(result.Token!, result.PaymentPageUrl!));
    }




    private static string GenerateOrderNumber() => $"LH-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

    private static OrderDetailResponse ToDetailResponse(Order order) => new(
    order.Id,
    order.OrderNumber,
    order.CreatedAtUtc,
    order.TotalAmount.Amount,
    order.Status,
    order.Items.Count,
    order.Address.ToString(),
    order.CustomerName,
    order.Phone,
    order.Items.Select(i => new OrderItemResponse(
        i.ProductId, i.ProductName, i.UnitPrice.Amount, i.Quantity, i.TotalAmount.Amount)).ToList());


    private static OrderSummaryResponse ToSummaryResponse(Order order) => new(
        order.Id,
        order.OrderNumber,
        order.CreatedAtUtc,
        order.TotalAmount.Amount,
        order.Status,
        order.Items.Count
        );


}

using FluentValidation;
using LootHub.Application.DTOs.Order;

namespace LootHub.Application.Validators;

public class OrderItemValidator : AbstractValidator<OrderItemRequest>
{
    public OrderItemValidator()
    {
        RuleFor(x => x.Quantity)
            .InclusiveBetween(1, 10);
        RuleFor(x => x.ProductId)
            .NotEmpty();
    }
}

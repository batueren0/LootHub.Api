using FluentValidation;
using LootHub.Application.DTOs.Order;

namespace LootHub.Application.Validators;

public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.Items)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(items => items.Select(i => i.ProductId).Distinct().Count() == items.Count)
            .WithMessage("Product list cannot contain the same product twice.");
        RuleFor(x => x.AddressId)
            .NotEmpty();

        RuleForEach(x => x.Items).SetValidator(new OrderItemValidator());
    }
}

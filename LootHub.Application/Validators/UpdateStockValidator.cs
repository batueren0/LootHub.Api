using FluentValidation;
using LootHub.Application.DTOs.Products;

namespace LootHub.Application.Validators;

public class UpdateStockValidator : AbstractValidator<UpdateStockRequest>
{
    public UpdateStockValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.Action)
            .IsInEnum();
    }
}

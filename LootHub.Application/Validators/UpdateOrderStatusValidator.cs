using FluentValidation;
using LootHub.Application.DTOs.Order;

namespace LootHub.Application.Validators;

public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum();
    }
}

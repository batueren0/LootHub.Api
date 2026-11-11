using FluentValidation;
using LootHub.Application.DTOs.Brand;

namespace LootHub.Application.Validators;

public class UpdateBrandValidator : AbstractValidator<UpdateBrandRequest>
{
    public UpdateBrandValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty()
           .MaximumLength(100);
    }
}

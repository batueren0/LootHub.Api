using FluentValidation;
using LootHub.Application.DTOs.Brand;

namespace LootHub.Application.Validators;

public class CreateBrandValidator : AbstractValidator<CreateBrandRequest>
{
    public CreateBrandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}

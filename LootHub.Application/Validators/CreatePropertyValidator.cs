using FluentValidation;
using LootHub.Application.DTOs.Property;

namespace LootHub.Application.Validators;

public class CreatePropertyValidator : AbstractValidator<CreatePropertyRequest>
{
    public CreatePropertyValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty()
           .MaximumLength(100);
    }
}

using FluentValidation;
using LootHub.Application.DTOs.Property;

namespace LootHub.Application.Validators;

public class UpdatePropertyValidator : AbstractValidator<UpdatePropertyRequest>
{
    public UpdatePropertyValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty()
           .MaximumLength(100);
    }
}

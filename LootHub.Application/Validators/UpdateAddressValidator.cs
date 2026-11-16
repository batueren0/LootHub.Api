using FluentValidation;
using LootHub.Application.DTOs.Address;

namespace LootHub.Application.Validators;

public class UpdateAddressValidator : AbstractValidator<UpdateAddressRequest>
{
    public UpdateAddressValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(x => x.Details)
            .NotEmpty()
            .MaximumLength(128);
        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(50);
    }
}

using FluentValidation;
using LootHub.Application.DTOs.Auth;

namespace LootHub.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(50)
            .NotEmpty();
        RuleFor(x => x.LastName)
            .MaximumLength(50)
            .NotEmpty();
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(256)
            .NotEmpty()
            .EmailAddress();
        RuleFor(x => x.Password)
            .MinimumLength(8)
            .NotEmpty();
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^5\d{9}$")
            .WithMessage("Phone number must be 10 digits and start with 5 (e.g. 5321234567).");
    }
}

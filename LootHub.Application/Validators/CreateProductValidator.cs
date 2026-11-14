using FluentValidation;
using LootHub.Application.DTOs.Products;

namespace LootHub.Application.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(128);
        RuleFor(x => x.Description)
            .NotEmpty()
            .Length(20, 4000);
        RuleFor(x => x.Price)
            .GreaterThan(0);
        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.ImageUrl)
            .Must(BeAValidUrl)
            .WithMessage("Image URL must be a valid http or https address.")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));
        RuleFor(x => x.BrandId)
            .NotEmpty();
        RuleFor(x => x.CategoryId)
            .NotEmpty();
        RuleFor(x => x.Properties)
            .Must(props => props.Select(p => p.PropertyId).Distinct().Count() == props.Count)
            .WithMessage("Properties list cannot contain the same property twice.");
        RuleForEach(x => x.Properties).ChildRules(property =>
        {
            property.RuleFor(p => p.PropertyId).NotEmpty();
            property.RuleFor(p => p.Value).NotEmpty().MaximumLength(150);
        });
    }

    private bool BeAValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}

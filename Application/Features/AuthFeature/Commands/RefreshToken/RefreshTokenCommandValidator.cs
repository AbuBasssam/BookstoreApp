using Application.Validations;
using ApplicationLayer.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;
namespace Application.Features.AuthFeature;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    private readonly IStringLocalizer<SharedResources> _Localizer;

    public RefreshTokenCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        _Localizer = localizer;
        ApplyValidationrules();

    }

    public void ApplyValidationrules()
    {
        RuleFor(x => x.AccessToken).ApplyNotEmptyRule(_Localizer).ApplyNotNullableRule(_Localizer);
    }
}

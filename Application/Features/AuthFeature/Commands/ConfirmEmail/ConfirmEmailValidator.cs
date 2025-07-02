using ApplicationLayer.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Features.AuthFeature;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
    private readonly IStringLocalizer<SharedResources> _Localizer;

    public ConfirmEmailValidator(IStringLocalizer<SharedResources> localizer)
    {
        _Localizer = localizer;

        _ApplyValidations();
    }

    private void _ApplyValidations()
    {

        RuleFor(x => x.ConfirmationCode)
            .NotEmpty()
            .WithMessage(_Localizer[SharedResorucesKeys.CodeRequired])

            .Length(6)
            .WithMessage(_Localizer[SharedResorucesKeys.InvalidCode]);
    }
}
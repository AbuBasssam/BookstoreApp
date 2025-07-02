using ApplicationLayer.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Features.AuthFeature;

public class VerifyResetPasswordCodeValidator : AbstractValidator<VerifyResetPasswordCodeCommand>
{
    private readonly IStringLocalizer<SharedResources> _Localizer;

    public VerifyResetPasswordCodeValidator(IStringLocalizer<SharedResources> localizer)
    {
        _Localizer = localizer;

        ApplyValidations();
    }

    private void ApplyValidations()
    {
        RuleFor(x => x.ConfirmationCode)
            .NotEmpty()
            .WithMessage(_Localizer[SharedResorucesKeys.CodeRequired])
            .Length(6).WithMessage(_Localizer[SharedResorucesKeys.InvalidCode]);
    }
}
using ApplicationLayer.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Features.AuthFeature;

public class SendResetPasswordCodeCommandValidator : AbstractValidator<SendResetPasswordCodeCommand>
{
    #region Field(s)

    private readonly IStringLocalizer<SharedResources> _Localizer;

    #endregion

    #region Constructor(s)
    public SendResetPasswordCodeCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        _Localizer = localizer;

        ApplyValidations();
    }
    #endregion

    #region Method(s)
    private void ApplyValidations()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(_Localizer[SharedResorucesKeys.EmailRequired])
            .EmailAddress().WithMessage(_Localizer[SharedResorucesKeys.InvalidEmail]);
    }
    #endregion
}

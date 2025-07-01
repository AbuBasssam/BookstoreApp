using ApplicationLayer.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Features.AuthFeature;

public class SendEmailConfirmationCodeValidator : AbstractValidator<SendEmailConfirmationCodeCommand>
{
    #region Field(s)

    IStringLocalizer<SharedResoruces> _localizer;

    #endregion

    #region Constructor(s)
    public SendEmailConfirmationCodeValidator(IStringLocalizer<SharedResoruces> localizer)
    {
        _localizer = localizer;
        ApplyValidations();

    }
    #endregion

    #region Method(s)
    private void ApplyValidations()
    {

        RuleFor(x => x.verificationToken)
            .NotEmpty()
            .WithMessage(_localizer[SharedResorucesKeys.TokenRequired]);

    }
    #endregion
}
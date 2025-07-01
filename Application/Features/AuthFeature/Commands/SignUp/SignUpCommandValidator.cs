﻿using ApplicationLayer.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Features.AuthFeature;

public class SignUpCommandValidator : AbstractValidator<SignUpCommand>
{
    #region Field(s)

    private readonly IStringLocalizer<SharedResoruces> _Localizer;

    #endregion

    #region Constructor(s)

    public SignUpCommandValidator(IStringLocalizer<SharedResoruces> Localizer)
    {
        _Localizer = Localizer;
        ApplyValidations();

    }
    #endregion

    #region Method(s)

    private void ApplyValidations()
    {
        RuleFor(x => x.Dto)
            .NotNull().WithMessage(_Localizer[SharedResorucesKeys.NameRequired])
            .SetValidator(new SignUpCommandDTO.Validator(_Localizer));
    }
    #endregion
}

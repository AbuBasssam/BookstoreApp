using Application.Validations;
using ApplicationLayer.Resources;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Features.AuthFeature;

public class SignUpCommandDTO
{

    #region Field(s)
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    #endregion

    #region Constructure(s)

    public SignUpCommandDTO(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;

    }

    #endregion

    #region Mapper(s)
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<SignUpCommandDTO, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

        }
    }
    #endregion

    #region Validation
    public class Validator : AbstractValidator<SignUpCommandDTO>
    {
        #region Field(s)
        private readonly IStringLocalizer<SharedResoruces> _Localizer;
        #endregion

        #region Constructure(s)
        public Validator(IStringLocalizer<SharedResoruces> localizer)
        {
            _Localizer = localizer;

            ApplyValidations();
        }
        #endregion

        #region Method(s)
        private void ApplyValidations()
        {
            RuleFor(x => x.Name)
                .ApplyCommonStringRules(8, 16, _Localizer)
                .WithMessage(_Localizer[SharedResorucesKeys.NameLengthRange])

                .NotEmpty()
                .WithMessage(_Localizer[SharedResorucesKeys.NameRequired])

                .Must(name => !name.Contains(' ')) // Reject any spaces
                .WithMessage(_Localizer[SharedResorucesKeys.NameNoSpacesAllowed])

                .Matches(@"^[a-zA-Z0-9]+$") // Only letters and numbers
                .WithMessage(_Localizer[SharedResorucesKeys.NameInvalidCharacters]);

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(_Localizer[SharedResorucesKeys.EmailRequired])

                .EmailAddress()
                .WithMessage(_Localizer[SharedResorucesKeys.InvalidEmail]);


        }
        #endregion
    }



    #endregion
}

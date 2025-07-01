using Application.Models;
using AutoMapper;
using Domain.Entities;
using Domain.HelperClasses;
using MediatR;

namespace Application.Features.AuthFeature;
public class SignInCommand : IRequest<Response<JwtAuthResult>>
{
    public string Email { get; set; }
    public string Password { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<SignInCommand, User>();
        }
    }

}

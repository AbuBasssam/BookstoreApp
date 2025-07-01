using Application.Interfaces;
using Application.Models;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.AuthFeature;

public class SignUpCommandHandler : IRequestHandler<SignUpCommand, Response<string>>
{
    #region Field(s)

    private readonly IAuthService _authService;
    private readonly IMapper _mapper;
    private readonly ResponseHandler _responseHandler;

    #endregion

    #region Constructure(s)
    public SignUpCommandHandler(IAuthService authService, IMapper mapper, ResponseHandler responseHandler)
    {
        _authService = authService;
        _mapper = mapper;
        _responseHandler = responseHandler;
    }
    #endregion

    #region Handler(s)
    public async Task<Response<string>> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        // map the DTO to User entity
        var newUser = _mapper.Map<User>(request.Dto);

        var result = await _authService.SignUp(newUser, request.Dto.Password);

        return result.IsSuccess ?
            _responseHandler.Success<string>(result.data) :
            _responseHandler.BadRequest<string>(string.Join('\n', result.Errors));


    }

    #endregion
}


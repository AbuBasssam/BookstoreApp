using Application.Features.AuthFeature;
using Domain.AppMetaData;
using Domain.HelperClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controller;
public class AuthController : ApiController
{
    [HttpPost(Router.AuthenticationRouter.SignIn)]
    [ProducesResponseType(typeof(JwtAuthResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn([FromBody] SignInCommand command)
    {
        var response = await Sender.Send(command);
        return NewResult(response);
    }


    [HttpPost(Router.AuthenticationRouter.SignUp)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp([FromBody] SignUpCommandDTO dto)
    {
        SignUpCommand command = new SignUpCommand(dto);
        var response = await Sender.Send(command);
        return NewResult(response);
    }


    [HttpPost(Router.AuthenticationRouter.ConfirmEmailCode)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [Authorize(Policy = Policies.VerificationOnly)]
    public async Task<IActionResult> SendEmailConfirmationOtp()
    {
        var verificationToken = HttpContext.GetAuthToken();

        // If the verification token is null or empty, return Unauthorized else execute the command
        return string.IsNullOrWhiteSpace(verificationToken) ? Unauthorized() : await _ExecuteCommand(verificationToken);


    }
    #region  Helpers

    private async Task<IActionResult> _ExecuteCommand(string verificationToken)
    {
        var command = new SendEmailConfirmationCodeCommand(verificationToken);

        var response = await Sender.Send(command);
        return NewResult(response);
    }

    #endregion
}
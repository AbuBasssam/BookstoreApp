﻿using Application.Features.AuthFeature;
using Application.Models;
using Domain.AppMetaData;
using Domain.HelperClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using Presentation.Helpers;

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

        return await CommandExecutor.Execute(
            command,
            Sender,
            (Response<JwtAuthResult> response) => NewResult(response)
        );
    }


    [HttpPost(Router.AuthenticationRouter.SignUp)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp([FromBody] SignUpCommandDTO dto)
    {

        return await CommandExecutor.Execute(
            new SignUpCommand(dto),
            Sender,
            (Response<string> response) => NewResult(response)
        );
    }


    [HttpPost(Router.AuthenticationRouter.ConfirmEmailCode)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [Authorize(Policy = Policies.VerificationOnly)]

    public async Task<IActionResult> SendEmailConfirmationOtp()
    {

        var verificationToken = HttpContext.GetAuthToken();

        return await CommandExecutor.Execute(
                new SendEmailConfirmationCodeCommand(verificationToken),
                Sender,
                (Response<string> response) => NewResult(response)
        );

    }


    [HttpPost(Router.AuthenticationRouter.VerifyEmailCode)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [Authorize(Policy = Policies.VerificationOnly)]
    public async Task<IActionResult> VerifyEmailConfirmationOtp([FromBody] ConfirmEmailOtpDto confirmationCode)
    {

        var verificationToken = HttpContext.GetAuthToken();

        return await CommandExecutor.Execute(
                new ConfirmEmailCommand { VerificationToken = verificationToken, ConfirmationCode = confirmationCode.ConfirmationCode },
                Sender,
                (Response<bool> response) => NewResult(response)
        );
    }


}
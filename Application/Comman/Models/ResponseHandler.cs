using ApplicationLayer.Resources;
using Microsoft.Extensions.Localization;
using System.Net;

namespace Application.Models;
public class ResponseHandler
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public ResponseHandler(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
    }
    private List<string> _ParseErrorMessages(string? message)
    {
        if (string.IsNullOrEmpty(message))
            return new List<string>();

        return message.Split(',', StringSplitOptions.RemoveEmptyEntries)
                     .Select(msg => msg.Trim())
                     .Where(msg => !string.IsNullOrEmpty(msg))
                     .ToList();
    }

    public Response<T> Deleted<T>(string? message = null)
    => new ResponseBuilder<T>()
            .WithStatusCode(HttpStatusCode.OK)
            .WithSuccess(true)
            .WithMessage(message ?? _stringLocalizer[SharedResorucesKeys.Deleted])
            .Build();


    public Response<T> Success<T>(T entity, object? meta = null)
        => new ResponseBuilder<T>()
            .WithStatusCode(HttpStatusCode.OK)
            .WithSuccess(true)
            .WithMessage(_stringLocalizer[SharedResorucesKeys.Success])
            .WithData(entity)
            .WithMeta(meta)
            .Build();
    public Response<T> Unauthorized<T>(string? message = null)
    {
        var errorMessages = _ParseErrorMessages(message);
        var defaultMessage = _stringLocalizer[SharedResorucesKeys.Unauthorized];

        return new ResponseBuilder<T>()
            .WithStatusCode(HttpStatusCode.Unauthorized)
            .WithSuccess(false)
            .WithMessage(errorMessages.Any() ? string.Join(", ", errorMessages) : defaultMessage)
            .WithErrors(errorMessages.Any() ? errorMessages : new List<string> { defaultMessage })
            .Build();
    }

    public Response<T> BadRequest<T>(string? message = null)
    {
        var errorMessages = _ParseErrorMessages(message);
        var defaultMessage = _stringLocalizer[SharedResorucesKeys.BadRequest];

        return new ResponseBuilder<T>()
            .WithStatusCode(HttpStatusCode.BadRequest)
            .WithSuccess(false)
            .WithMessage(errorMessages.Any() ? string.Join(", ", errorMessages) : defaultMessage)
            .WithErrors(errorMessages.Any() ? errorMessages : new List<string> { defaultMessage })
            .Build();
    }

    public Response<T> UnprocessableEntity<T>(string? message = null)
    {
        var errorMessages = _ParseErrorMessages(message);
        var defaultMessage = _stringLocalizer[SharedResorucesKeys.UnprocessableEntity];

        return new ResponseBuilder<T>()
            .WithStatusCode(HttpStatusCode.UnprocessableEntity)
            .WithSuccess(false)
            .WithMessage(errorMessages.Any() ? string.Join(", ", errorMessages) : defaultMessage)
            .WithErrors(errorMessages.Any() ? errorMessages : new List<string> { defaultMessage })
            .Build();
    }

    public Response<T> NotFound<T>(string? message = null)
    {
        var errorMessages = _ParseErrorMessages(message);
        var defaultMessage = _stringLocalizer[SharedResorucesKeys.NotFound];

        return new ResponseBuilder<T>()
            .WithStatusCode(HttpStatusCode.NotFound)
            .WithSuccess(false)
            .WithMessage(errorMessages.Any() ? string.Join(", ", errorMessages) : defaultMessage)
            .WithErrors(errorMessages.Any() ? errorMessages : new List<string> { defaultMessage })
            .Build();
    }

    public Response<T> InternalServerError<T>(string? message = null)
    {
        var errorMessages = _ParseErrorMessages(message);
        var defaultMessage = _stringLocalizer[SharedResorucesKeys.InternalServerError];

        return new ResponseBuilder<T>()
            .WithStatusCode(HttpStatusCode.InternalServerError)
            .WithSuccess(false)
            .WithMessage(errorMessages.Any() ? string.Join(", ", errorMessages) : defaultMessage)
            .WithErrors(errorMessages.Any() ? errorMessages : new List<string> { defaultMessage })
            .Build();
    }


    public Response<T> Created<T>(T entity, object? meta = null)
        => new ResponseBuilder<T>()
            .WithStatusCode(HttpStatusCode.Created)
            .WithSuccess(true)
            .WithMessage(_stringLocalizer[SharedResorucesKeys.Created])
            .WithData(entity)
            .WithMeta(meta)
            .Build();


}

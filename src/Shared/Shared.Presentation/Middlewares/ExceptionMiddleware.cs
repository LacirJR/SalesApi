using System.Net;
using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Module.Users.Domain.Exceptions;
using Shared.Application.Exceptions;
using Shared.Domain.Common;

namespace Shared.Presentation.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        ServiceError errorResponse;
        int statusCode;

        switch (exception)
        {
            case ServiceResultException sr:
                statusCode = StatusCodes.Status400BadRequest;
                errorResponse = sr.ServiceError;
                break;
            
            case ValidationException ve:
                statusCode = (int)HttpStatusCode.BadRequest;
                errorResponse = ServiceError.ModelStateError(FormatValidationErrors(ve.Errors));
                break;

            case UserDomainException de:
                statusCode = (int)HttpStatusCode.UnprocessableEntity;
                errorResponse = ServiceError.GenericError("UserDomainError", de.Message, "A business rule was violated.");
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse = ServiceError.DefaultError;
                break;
        }

        response.StatusCode = statusCode;
        return response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }

    private static string FormatValidationErrors(IEnumerable<ValidationFailure> errors)
    {
        return string.Join("; ", errors.Select(e => $"{e.PropertyName}: {string.Join(", ", e.ErrorMessage)}"));
    }
}
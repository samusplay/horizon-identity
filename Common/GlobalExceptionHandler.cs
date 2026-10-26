// Common/GlobalExceptionHandler.cs
using Horizon.Identity.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Horizon.Identity.Common;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Error de validación"),
            InvalidCredentialsException => (StatusCodes.Status401Unauthorized, "Credenciales inválidas"),
            InvalidResetTokenException => (StatusCodes.Status400BadRequest, "Enlace inválido"),
            EmailAlreadyExistsException => (StatusCodes.Status409Conflict, "Conflicto"),
            _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
        };

        // Los 4xx son peticiones rechazadas esperadas; no deben llenar el log de errores.
        if (statusCode >= StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Excepción no controlada");
        else
            logger.LogWarning("Petición rechazada ({StatusCode}): {Message}", statusCode, exception.Message);

        httpContext.Response.StatusCode = statusCode;

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message
        };

        if (exception is ValidationException validationException)
        {
            problem.Extensions["errors"] = validationException.Errors.Select(e => e.ErrorMessage);
        }

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
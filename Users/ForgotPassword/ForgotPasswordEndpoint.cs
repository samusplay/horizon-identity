namespace Horizon.Identity.Users.ForgotPassword;

public record ForgotPasswordRequest(string Email);

public record ForgotPasswordResponse(string Message);

public class ForgotPasswordEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/users/forgot-password",
            async (ForgotPasswordRequest request, ISender sender) =>
            {
                var command = request.Adapt<ForgotPasswordCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<ForgotPasswordResponse>();

                return Results.Ok(response);
            })
        .WithName("ForgotPassword")
        .Produces<ForgotPasswordResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Solicita un enlace de recuperación de contraseña")
        .WithDescription("Si el correo está registrado, envía un enlace de un solo uso válido por 30 minutos.")
        .WithTags("Users");
    }
}

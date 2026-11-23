namespace Horizon.Identity.Users.ForgotPassword;

// Record de solicitud del cliente
public record ForgotPasswordRequest(string Email);

// Respuesta hacia el cliente
public record ForgotPasswordResponse(string Message);

public class ForgotPasswordEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/users/forgot-password",
            async (ForgotPasswordRequest request, ISender sender) =>
            {
                // Adaptamos el request a nuestro command
                var command = request.Adapt<ForgotPasswordCommand>();

                // Lo enviamos a su respectivo Handler (pasando primero por ValidationBehavior)
                var result = await sender.Send(command);
                var response = result.Adapt<ForgotPasswordResponse>();

                return Results.Ok(response);
            })
        .WithName("ForgotPassword")
        .Produces<ForgotPasswordResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Solicita un enlace de recuperación de contraseña")
        .WithTags("Users");
    }
}
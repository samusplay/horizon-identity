namespace Horizon.Identity.Users.ResetPassword;

// Record de solicitud del cliente
public record ResetPasswordRequest(
    string Token,
    string NewPassword
);

// Respuesta hacia el cliente
public record ResetPasswordResponse(string Message);

public class ResetPasswordEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/users/reset-password",
            async (ResetPasswordRequest request, ISender sender) =>
            {
                // Adaptamos el request a nuestro command
                var command = request.Adapt<ResetPasswordCommand>();

                // Lo enviamos a su respectivo Handler (pasando primero por ValidationBehavior)
                var result = await sender.Send(command);
                var response = result.Adapt<ResetPasswordResponse>();

                return Results.Ok(response);
            })
        .WithName("ResetPassword")
        .Produces<ResetPasswordResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Restablece la contraseña utilizando el token de recuperación")
        .WithTags("Users");
    }
}
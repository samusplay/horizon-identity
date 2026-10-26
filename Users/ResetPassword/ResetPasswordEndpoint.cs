namespace Horizon.Identity.Users.ResetPassword;

public record ResetPasswordRequest(string Token, string NewPassword);

public record ResetPasswordResponse(string Message);

public class ResetPasswordEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/users/reset-password",
            async (ResetPasswordRequest request, ISender sender) =>
            {
                var command = request.Adapt<ResetPasswordCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<ResetPasswordResponse>();

                return Results.Ok(response);
            })
        .WithName("ResetPassword")
        .Produces<ResetPasswordResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Restablece la contraseña con un token de recuperación")
        .WithDescription("Valida el token de un solo uso y actualiza la contraseña del usuario.")
        .WithTags("Users");
    }
}

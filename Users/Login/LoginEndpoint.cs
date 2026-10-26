namespace Horizon.Identity.Users.Login;

// Lo que envía el cliente.
public record LoginRequest(
    string Email,
    string Password
);

// Lo que devolvemos: el frontend espera exactamente { "token": "..." }.
public record LoginResponse(string Token);

public class LoginEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/users/login",
            async (LoginRequest request, ISender sender) =>
            {
                var command = request.Adapt<LoginUserCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<LoginResponse>();

                return Results.Ok(response);
            })
        .WithName("LoginUser")
        .Produces<LoginResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Inicia sesión y devuelve un token JWT")
        .WithDescription("Valida el correo y la contraseña del usuario y emite un JWT válido por 1 hora.")
        .WithTags("Users");
    }
}

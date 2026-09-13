namespace Horizon.Identity.Users.Login;

//Record de Creacion del Usuario 
public record LoginRequest(string Email, string Password);
//Respuesta
public record LoginResponse(string Token);

public class LoginEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/users/login",
            async (LoginRequest request, ISender sender) =>
            {
                //Adaptamos el request a nuestro command
                var command = request.Adapt<LoginCommand>();
                //Lo enviamos a su respectivo Handler
                var result = await sender.Send(command);
                var response = result.Adapt<LoginResponse>();

                return Results.Ok(response);
            })
        .WithName("LoginUser")
        .Produces<LoginResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Inicia sesión y devuelve un JWT")
        .WithTags("Users");
    }
}
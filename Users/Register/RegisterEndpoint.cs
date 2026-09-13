using Carter;
using Mapster;
using MediatR;

namespace Horizon.Identity.Users.Register;
//Luego de Hacer la logica del Handler y validacion
//pasamos al ednpoint donde recibira la peticion HTTP

// Definimos el Record Lo que va enviar el Cliente
public record RegisterRequest(
    string Email,
    string Password,
    string Role
);
//lo que vamos devolver al cliente
public record RegisterResponse(Guid Id);

//Heredamos Carter para reunir los endpoints
public class RegisterEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        //Definimos  la ruta
        //Isender actuad e mensajero con el handler corresponiente al command  o query

        app.MapPost("/api/identity/users/register",
            async (RegisterRequest request, ISender sender) =>
            {
                //Mapeo Request -> Command usando Mapster
                var command = request.Adapt<RegisterUserCommand>();

                //MediatR manda el comando al Handler (pasando primero por ValidationBehavior)
                var result = await sender.Send(command);

                //Mapeo Result -> Response usando Mapster
                var response = result.Adapt<RegisterResponse>();

                //201 Created con la ubicación del recurso creado
                return Results.Created($"/api/identity/users/{response.Id}", response);
            })
        .WithName("RegisterUser")
        .Produces<RegisterResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Registra un nuevo usuario")
        .WithDescription("Crea un usuario validando unicidad de correo y hasheando la contraseña.")
        .WithTags("Users");
    }

}


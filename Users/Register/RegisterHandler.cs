
using Horizon.Identity.Common.Exceptions;

namespace Horizon.Identity.Users.Register;

//Empezar a programar desde el handler va manejar la logica de Negocio
//1.Definir Records una estrctura para transportar datos inmutable
//Utilizamos Command y Query ya que utilizamos Patrom CQRS
//Comand es Post,Put,Delete lo que es Modicar Datos
//Query es consulta
//La Carpeta Cqrs ya tiene las firmas a implementar no editar la Carpeta CQRS

public record RegisterUserCommand(
    //Lo campos con los que vamos crear ese usuario
    string Email,
    string Password,
    string Role
    //Utilizamos Types para  saber que tipo de dato de de respuesta
    ) : ICommand<RegisterUserResult>;

//Record de Resultado
public record RegisterUserResult(Guid Id);

//Esta Clase la hacemos privada para manejar la logica
//Heredamos para comand su  firma de handler para asegurar la salida Typada
//un parametro de creacion y de Respuesta

// aqui inyectamos las herramientas que necesitamos para hablar con la DB
//(IdentityDbContext dbContext)
internal class RegisterHandler(IdentityDbContext dbContext)
    : ICommandHandler<RegisterUserCommand, RegisterUserResult>
{
    //Implementamos la Interfaz
    public async Task<RegisterUserResult> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        //Aqui dentro del metodo vamos manejar la logica

        //Normalizamos el correo para que login y registro usen el mismo formato.
        var email = command.Email.Trim().ToLowerInvariant();

        //Buscamos si el email Existe
        var emailExists = await dbContext.Users
            .AnyAsync(u => u.Email == email, cancellationToken);

        if (emailExists)
            throw new EmailAlreadyExistsException(email);

        //Encriptamos la contraseña
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);

        //Creamos el Nuevo Usuario
        var user = new User
        {
            Email = email,
            PasswordHash = passwordHash,
            Role = Enum.Parse<UserRole>(command.Role)

        };
        //Guardamos en la Base de datos
        dbContext.Users.Add(user);
        //Por si no se llega dar la respuesta de la base de datos
        await dbContext.SaveChangesAsync(cancellationToken);

        //Retornamos el Resultado segun la firma

        return new RegisterUserResult(user.Id);


    }
}
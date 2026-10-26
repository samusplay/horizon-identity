using Horizon.Identity.Common.Auth;
using Horizon.Identity.Common.Exceptions;

namespace Horizon.Identity.Users.Login;

// Command (CQRS): credenciales que llegan desde el endpoint.
public record LoginUserCommand(
    string Email,
    string Password
) : ICommand<LoginUserResult>;

// Resultado: el token que el frontend guarda en la cookie httpOnly.
public record LoginUserResult(string Token);

internal class LoginHandler(IdentityDbContext dbContext, ITokenProvider tokenProvider)
    : ICommandHandler<LoginUserCommand, LoginUserResult>
{
    public async Task<LoginUserResult> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        // Normalizamos igual que en el registro para que el correo coincida
        // aunque el usuario escriba mayúsculas o espacios.
        var email = command.Email.Trim().ToLowerInvariant();

        var user = await dbContext.Users
            .SingleOrDefaultAsync(u => u.Email == email, cancellationToken);

        // Mismo error para correo inexistente y contraseña incorrecta.
        if (user is null)
            throw new InvalidCredentialsException();

        if (!BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        var token = tokenProvider.Create(user);

        return new LoginUserResult(token);
    }
}

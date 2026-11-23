using Horizon.Identity.Common.Exceptions;
using Horizon.Identity.Common.Security;

namespace Horizon.Identity.Users.ResetPassword;

// El Comando
public record ResetPasswordCommand(
    string Token,
    string NewPassword
) : ICommand<ResetPasswordResult>;

// El Resultado
public record ResetPasswordResult(string Message);

// El Handler
internal class ResetPasswordHandler(IdentityDbContext dbContext)
    : ICommandHandler<ResetPasswordCommand, ResetPasswordResult>
{
    public async Task<ResetPasswordResult> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        // 1. Calculamos el hash SHA-256 del token recibido para buscarlo en la base de datos
        var tokenHash = ResetToken.Hash(command.Token);

        // 2. Buscamos el registro del token junto con el usuario asociado
        var resetToken = await dbContext.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        // 3. Validamos que exista, no haya sido usado y no esté vencido
        if (resetToken is null || !resetToken.IsUsable(now))
            throw new InvalidResetTokenException();

        // 4. Actualizamos la contraseña con BCrypt y quemamos el token (un solo uso)
        resetToken.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.NewPassword);
        resetToken.MarkAsUsed(now);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ResetPasswordResult("Contraseña actualizada correctamente.");
    }
}
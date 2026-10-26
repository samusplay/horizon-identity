using Horizon.Identity.Common.Auth;
using Horizon.Identity.Common.Exceptions;

namespace Horizon.Identity.Users.ResetPassword;

public record ResetPasswordCommand(string Token, string NewPassword) : ICommand<ResetPasswordResult>;

public record ResetPasswordResult(string Message);

internal class ResetPasswordHandler(IdentityDbContext dbContext)
    : ICommandHandler<ResetPasswordCommand, ResetPasswordResult>
{
    public async Task<ResetPasswordResult> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var tokenHash = ResetTokenHasher.Hash(command.Token.Trim());

        var resetToken = await dbContext.PasswordResetTokens
            .Include(t => t.User)
            .SingleOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        if (resetToken is null || resetToken.UsedAt is not null || resetToken.ExpiresAt <= DateTime.UtcNow)
            throw new InvalidResetTokenException();

        resetToken.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.NewPassword);
        resetToken.UsedAt = DateTime.UtcNow;

        // Cualquier otro enlace pendiente del mismo usuario deja de ser válido.
        var otherTokens = await dbContext.PasswordResetTokens
            .Where(t => t.UserId == resetToken.UserId && t.Id != resetToken.Id)
            .ToListAsync(cancellationToken);

        dbContext.PasswordResetTokens.RemoveRange(otherTokens);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ResetPasswordResult("Contraseña actualizada correctamente.");
    }
}

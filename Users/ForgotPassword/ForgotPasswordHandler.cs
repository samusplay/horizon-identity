using Horizon.Identity.Common.Messaging;
using Horizon.Identity.Common.Security;

namespace Horizon.Identity.Users.ForgotPassword;

// El Comando
public record ForgotPasswordCommand(string Email) : ICommand<ForgotPasswordResult>;

// El Resultado
public record ForgotPasswordResult(string Message);

// El Handler
internal class ForgotPasswordHandler(
    IdentityDbContext dbContext,
    IEventPublisher eventPublisher,
    IConfiguration configuration
) : ICommandHandler<ForgotPasswordCommand, ForgotPasswordResult>
{
    public async Task<ForgotPasswordResult> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        const string responseMessage =
            "Si el correo existe en Horizon, recibirás un enlace para restablecer tu contraseña.";

        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        // Respuesta genérica para no revelar si el correo está registrado
        if (user is null)
            return new ForgotPasswordResult(responseMessage);

        var now = DateTime.UtcNow;

        // Generamos el token plano (va en el enlace) y guardamos solo su hash SHA-256
        // Generamos el token plano (va en el enlace) y su hash SHA-256 (va a la base)
        var (token, tokenHash) = ResetToken.Generate();
        var resetToken = PasswordResetToken.Create(user.Id, tokenHash, now);
        dbContext.PasswordResetTokens.Add(resetToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var frontendUrl = (configuration["Frontend:BaseUrl"] ?? "http://localhost:3000").TrimEnd('/');
        var resetLink = $"{frontendUrl}/reset-password?token={token}";

        var integrationEvent = new PasswordResetRequestedEvent(
            EventId: Guid.NewGuid(),
            OccurredAt: now,
            UserId: user.Id,
            Email: user.Email,
            ResetUrl: resetLink,
            ExpiresAt: resetToken.ExpiresAt);

        await eventPublisher.PublishAsync(
            PasswordResetRequestedEvent.RoutingKey,
            integrationEvent,
            cancellationToken);

        return new ForgotPasswordResult(responseMessage);
    }
}
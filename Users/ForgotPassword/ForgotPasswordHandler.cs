using System.Security.Cryptography;
using Horizon.Identity.Common.Auth;
using Horizon.Identity.Common.Email;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Horizon.Identity.Users.ForgotPassword;

public record ForgotPasswordCommand(string Email) : ICommand<ForgotPasswordResult>;

public record ForgotPasswordResult(string Message);

internal class ForgotPasswordHandler(
    IdentityDbContext dbContext,
    IEmailSender emailSender,
    IOptions<PasswordResetOptions> options,
    ILogger<ForgotPasswordHandler> logger)
    : ICommandHandler<ForgotPasswordCommand, ForgotPasswordResult>
{
    // Misma respuesta exista o no el correo: evita enumerar usuarios.
    private const string GenericMessage =
        "Si el correo está registrado, recibirás un enlace para restablecer tu contraseña.";

    public async Task<ForgotPasswordResult> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        var email = command.Email.Trim().ToLowerInvariant();

        var user = await dbContext.Users
            .SingleOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null)
            return new ForgotPasswordResult(GenericMessage);

        // Un solo enlace activo por usuario: invalidamos los anteriores.
        var previousTokens = await dbContext.PasswordResetTokens
            .Where(t => t.UserId == user.Id)
            .ToListAsync(cancellationToken);

        dbContext.PasswordResetTokens.RemoveRange(previousTokens);

        // Token de 32 bytes criptográficos, seguro para URL.
        var rawToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));

        dbContext.PasswordResetTokens.Add(new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = ResetTokenHasher.Hash(rawToken),
            ExpiresAt = DateTime.UtcNow.AddMinutes(options.Value.ExpiryMinutes),
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        var resetLink =
            $"{options.Value.FrontendBaseUrl.TrimEnd('/')}/reset-password?token={Uri.EscapeDataString(rawToken)}";

        try
        {
            await emailSender.SendPasswordResetAsync(user.Email, resetLink, cancellationToken);
        }
        catch (Exception exception)
        {
            // No revelamos el fallo al cliente; el usuario puede volver a intentarlo.
            logger.LogError(exception, "No se pudo enviar el correo de recuperación a {Email}", user.Email);
        }

        return new ForgotPasswordResult(GenericMessage);
    }
}

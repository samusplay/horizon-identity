using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Horizon.Identity.Common.Email;

// Envía correos por SMTP. Si Email:Enabled es false (desarrollo) no envía
// nada y deja el enlace en los logs para poder probar el flujo.
public class SmtpEmailSender(
    IOptions<EmailOptions> options,
    IHostEnvironment environment,
    ILogger<SmtpEmailSender> logger) : IEmailSender
{
    private readonly EmailOptions _options = options.Value;

    public async Task SendPasswordResetAsync(string toEmail, string resetLink, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            if (environment.IsDevelopment())
            {
                // Solo en desarrollo: permite probar el flujo sin SMTP.
                logger.LogWarning(
                    "Correo deshabilitado (Email:Enabled=false). Enlace de recuperación para {Email}: {ResetLink}",
                    toEmail,
                    resetLink);
            }
            else
            {
                // Nunca loguear el token fuera de desarrollo.
                logger.LogError(
                    "Email:Enabled=false. No se envió el correo de recuperación a {Email}.",
                    toEmail);
            }

            return;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(_options.FromAddress, _options.FromName),
            Subject = "Recupera tu contraseña de Horizon",
            Body = BuildBody(resetLink),
            IsBodyHtml = true,
        };
        message.To.Add(toEmail);

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.UseSsl,
            Credentials = new NetworkCredential(_options.UserName, _options.Password),
        };

        await client.SendMailAsync(message, cancellationToken);
    }

    private static string BuildBody(string resetLink) => $"""
        <p>Recibimos una solicitud para restablecer tu contraseña de Horizon.</p>
        <p><a href="{resetLink}">Restablecer contraseña</a></p>
        <p>El enlace expira en 30 minutos. Si no lo solicitaste, ignora este correo.</p>
        """;
}

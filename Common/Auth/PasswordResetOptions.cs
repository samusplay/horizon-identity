namespace Horizon.Identity.Common.Auth;

// Configuración del flujo de recuperación de contraseña.
public class PasswordResetOptions
{
    public const string SectionName = "PasswordReset";

    // Minutos de validez del enlace enviado por correo.
    public int ExpiryMinutes { get; set; } = 30;

    // URL pública del frontend para construir el enlace del correo.
    public string FrontendBaseUrl { get; set; } = string.Empty;
}

namespace Horizon.Identity.Common.Auth;

// Configuración del token JWT. Se enlaza desde la sección "Jwt" de appsettings.
// El SecretKey NUNCA debe quedar hardcodeado en producción: se inyecta por
// variable de entorno (Jwt__SecretKey) o user-secrets.
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "Horizon.Identity";
    public string Audience { get; set; } = "Horizon.Client";
    public string SecretKey { get; set; } = string.Empty;

    // Debe coincidir con el maxAge de la cookie del frontend (1 hora).
    public int ExpiryMinutes { get; set; } = 60;
}

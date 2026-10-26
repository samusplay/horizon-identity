namespace Horizon.Identity.Common.Email;

// Configuración SMTP. Si Enabled = false, el enlace se escribe en el log
// (modo desarrollo) en lugar de enviarse por correo.
public class EmailOptions
{
    public const string SectionName = "Email";

    public bool Enabled { get; set; }
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromAddress { get; set; } = "no-reply@horizon.dev";
    public string FromName { get; set; } = "Horizon";
}

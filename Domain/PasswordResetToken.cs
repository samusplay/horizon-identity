namespace Horizon.Identity.Domain;

// Token de un solo uso para restablecer la contraseña.
// Nunca se guarda el token en claro: solo su hash SHA-256.
public class PasswordResetToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UsedAt { get; set; }

    public User User { get; set; } = default!;
}

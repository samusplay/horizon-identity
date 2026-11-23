namespace Horizon.Identity.Domain;

// Enlace de recuperación de contraseña: un solo uso y con vencimiento.
// El token viaja por correo y NUNCA se guarda: en la base solo va su hash SHA-256.
//una relacion de 1 a muchos
public class PasswordResetToken
{
    // cuánto dura un enlace de recuperación (corto: da acceso a la cuenta)
    public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(30);

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UsedAt { get; private set; }

    public User User { get; private set; } = default!;

    private PasswordResetToken() { } // para EF Core

    public static PasswordResetToken Create(Guid userId, string tokenHash, DateTime nowUtc) => new()
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        TokenHash = tokenHash,
        CreatedAt = nowUtc,
        ExpiresAt = nowUtc + Lifetime,
    };

    // ¿todavía sirve? (no usado y no vencido)
    public bool IsUsable(DateTime nowUtc) => UsedAt is null && ExpiresAt > nowUtc;

    // se gasta al cambiar la contraseña
    public void MarkAsUsed(DateTime nowUtc) => UsedAt = nowUtc;
}
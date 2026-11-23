namespace Horizon.Identity.Common.Messaging;

public sealed record PasswordResetRequestedEvent(
    Guid EventId,
    DateTime OccurredAt,
    Guid UserId,
    string Email,
    string ResetUrl,
    DateTime ExpiresAt)
{
    public const string RoutingKey = "identity.password-reset-requested";
}
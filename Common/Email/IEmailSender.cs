namespace Horizon.Identity.Common.Email;

public interface IEmailSender
{
    Task SendPasswordResetAsync(string toEmail, string resetLink, CancellationToken cancellationToken = default);
}

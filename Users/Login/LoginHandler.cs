using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Horizon.Identity.Users.Login;

//El Comando
public record LoginCommand(
    string Email,
    string Password
) : ICommand<LoginResult>;

//El Resultado
public record LoginResult(string Token);

//El Handler
internal class LoginHandler(IdentityDbContext dbContext, IConfiguration configuration)
    : ICommandHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        var token = GenerateToken(user);

        return new LoginResult(token);
    }

    private string GenerateToken(User user)
    {
        var privateKeyPath = Path.Combine(AppContext.BaseDirectory, "Security", "Keys", "private.pem");
        var rsa = RSA.Create();
        rsa.ImportFromPem(File.ReadAllText(privateKeyPath));

        var signingCredentials = new SigningCredentials(
            new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);

        var claims = new Dictionary<string, object>
        {
            [JwtRegisteredClaimNames.Sub] = user.Id.ToString(),
            [ClaimTypes.Role] = user.Role.ToString(),
            [JwtRegisteredClaimNames.Email] = user.Email
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
            Claims = claims,
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = signingCredentials
        };

        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(descriptor);
    }
}
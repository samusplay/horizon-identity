using System.Security.Cryptography;
using System.Text;

namespace Horizon.Identity.Common.Auth;

// El enlace viaja en claro al usuario; en la base de datos solo guardamos
// el hash SHA-256 para que una filtración no permita reutilizar tokens.
public static class ResetTokenHasher
{
    public static string Hash(string token)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}

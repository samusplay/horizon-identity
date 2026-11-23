using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace Horizon.Identity.Common.Security;

// Genera el token secreto del enlace de recuperación.
// Regla: el TOKEN viaja por correo y NUNCA se guarda; en la base solo va su HASH.
public static class ResetToken
{
    // Devuelve los dos valores a la vez: (Token, Hash)
    public static (string Token, string Hash) Generate()
    {
        // 32 bytes aleatorios seguros (256 bits), en Base64Url para que viaje en una URL
        var token = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
        return (token, Hash(token));
    }

    // Huella SHA-256 en hexadecimal (64 caracteres): se usa al crear y al buscar
    public static string Hash(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
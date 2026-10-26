namespace Horizon.Identity.Common.Exceptions;

// Token inexistente, expirado o ya utilizado.
public class InvalidResetTokenException()
    : Exception("El enlace de recuperación es inválido o ha expirado.");

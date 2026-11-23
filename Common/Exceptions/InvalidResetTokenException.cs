// Common/Exceptions/InvalidResetTokenException.cs
namespace Horizon.Identity.Common.Exceptions;

public class InvalidResetTokenException()
    : Exception("El enlace de recuperación es inválido o ya expiró.");
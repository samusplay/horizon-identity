// Common/Exceptions/EmailAlreadyExistsException.cs
namespace Horizon.Identity.Common.Exceptions;

public class EmailAlreadyExistsException(string email)
    : Exception($"El correo '{email}' ya está registrado.");
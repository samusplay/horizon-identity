namespace Horizon.Identity.Common.Exceptions;

// Se lanza tanto si el correo no existe como si la contraseña es incorrecta.
// El mensaje es siempre el mismo para no revelar qué correos están registrados.
public class InvalidCredentialsException()
    : Exception("El correo o la contraseña son incorrectos.");

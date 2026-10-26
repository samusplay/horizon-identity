namespace Horizon.Identity.Common.Auth;

// Abstracción para emitir tokens. Así el handler de login no depende de JWT
// y se puede sustituir en pruebas.
public interface ITokenProvider
{
    string Create(User user);
}

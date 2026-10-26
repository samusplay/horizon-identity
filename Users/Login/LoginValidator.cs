namespace Horizon.Identity.Users.Login;

public class LoginValidator : AbstractValidator<LoginUserCommand>
{
    public LoginValidator()
    {
        // En login solo validamos la forma; la contraseña real se comprueba
        // en el handler con BCrypt.
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo es requerido.")
            .EmailAddress().WithMessage("Formato de correo inválido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida.");
    }
}

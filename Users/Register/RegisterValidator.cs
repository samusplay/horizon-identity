

namespace Horizon.Identity.Users.Register;

//Reglas para  validar los datos de entrda del Usuario
//Utilizamos el Record de Creacion
public class RegisterValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterValidator()
    {
        //Cada Regla valida un campo
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo es requerido.")
            .EmailAddress().WithMessage("Formato de correo inválido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida.")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");

        RuleFor(x => x.Role)
            .Must(role => role == "Espectador" || role == "Creador")
            .WithMessage("El rol debe ser 'Espectador' o 'Creador'.");
    }
}
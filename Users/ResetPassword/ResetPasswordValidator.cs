namespace Horizon.Identity.Users.ResetPassword;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("El enlace de recuperación es inválido.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("La contraseña es requerida.")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.")
            .MaximumLength(128).WithMessage("La contraseña no puede superar los 128 caracteres.");
    }
}

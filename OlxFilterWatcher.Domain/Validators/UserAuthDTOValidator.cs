namespace OlxFilterWatcher.Domain.Validators;

public class UserAuthDTOValidator : AbstractValidator<UserAuthDTO>
{
    public UserAuthDTOValidator()
    {
        RuleFor(x => x)
            .NotNull()
            .WithMessage("Credencial inválida.");

        RuleFor(x => x.Email)
            .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible)
            .WithMessage("Formato de e-mail inválido.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .Length(8, 30)
            .WithMessage("A senha precisa ter entre 8 e 30 caracteres")
            .Must(VerifyPassword)
            .WithMessage("A senha precisa conter ao menos 1 dígito, 1 letra maiúscula e 1 minúscula");
    }

    private bool VerifyPassword(string password)
    {
        return password.Any(c => char.IsDigit(c)) &
               password.Any(c => char.IsLetter(c)) &
               password.Any(c => char.IsUpper(c));
    }
}
using ClinicalTrialsApi.Models.Dtos;
using FluentValidation;

namespace ClinicalTrialsApi.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio")
                .EmailAddress().WithMessage("Formato de email inválido");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria")
                .MinimumLength(4).WithMessage("Debe tener al menos 4 caracteres");
        }
    }
}

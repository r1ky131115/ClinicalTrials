using ClinicalTrialsApi.Models.Dtos;
using FluentValidation;

namespace ClinicalTrialsApi.Validators
{
    public class CreateClinicalTrialDtoValidator : AbstractValidator<CreateClinicalTrialDto>
    {
        private static readonly string[] ValidPhases = {"I", "II", "III", "IV"};
        private static readonly string[] ValidStatuses = { "Recruiting", "Active", "Completed", "Cancelled"};

        public CreateClinicalTrialDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.")
                .Matches(@"^[A-Z0-9-]+$").WithMessage("El nombre debe ser código en mayúsculas, números y guiones (ej: GSK-ASTHMA-2026-A).");

            RuleFor(x => x.Phase)
                .NotEmpty()
                .Must(phase => ValidPhases.Contains(phase)).WithMessage($"La fase debe ser una de: {string.Join(", ", ValidPhases)}");

            RuleFor(x => x.PatientCount)
                .GreaterThan(0).WithMessage("El número de pacientes debe ser mayor que cero.")
                .LessThanOrEqualTo(100_000).WithMessage("El número de pacientes no puede exceder los 100,000.");
            
            RuleFor(x => x.Status)
                .NotEmpty()
                .Must(status => ValidStatuses.Contains(status)).WithMessage($"El estado debe ser uno de: {string.Join(", ", ValidStatuses)}");

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .GreaterThan(new DateTime(2000, 1, 1)).WithMessage("La fecha de inicio debe ser posterior al 1 de enero de 2000.");

            // Regla condicional: Si Phase es III, PatientCount minimo es 100
            RuleFor(x => x.PatientCount)
                .GreaterThanOrEqualTo(100)
                .When(x => x.Phase == "III")
                .WithMessage("Los estudios Fase III requieren al menos 100 pacientes.");
        }
    }
}
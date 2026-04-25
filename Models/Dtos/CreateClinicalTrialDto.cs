namespace ClinicalTrialsApi.Models.Dtos
{
    public record CreateClinicalTrialDto(
        string Name,
        string Phase,
        int PatientCount,
        string Status,
        DateTime StartDate
    );
}
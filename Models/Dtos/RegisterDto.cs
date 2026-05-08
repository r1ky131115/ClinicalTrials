namespace ClinicalTrialsApi.Models.Dtos
{
    public record RegisterDto(
        string Email, 
        string Password, 
        string FullName
    );
}
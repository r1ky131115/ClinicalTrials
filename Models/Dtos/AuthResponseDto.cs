namespace ClinicalTrialsApi.Models.Dtos
{
    public record AuthResponseDto(
        string Token, 
        string Email, 
        DateTime Expiration
    );
}
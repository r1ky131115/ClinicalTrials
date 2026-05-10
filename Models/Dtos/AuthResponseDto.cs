namespace ClinicalTrialsApi.Models.Dtos
{
    public record AuthResponseDto(
        string Token, 
        string UserId,
        string UserName,
        string Email, 
        DateTime ExpirationAt
    );
}
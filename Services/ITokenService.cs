using Microsoft.AspNetCore.Identity;

namespace ClinicalTrialsApi.Services;

public interface ITokenService
{
    string CreateToken(IdentityUser user);
}
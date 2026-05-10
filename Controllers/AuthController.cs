using ClinicalTrialsApi.Models.Dtos;
using ClinicalTrialsApi.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalTrialsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IValidator<RegisterDto> _createValidator;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<IdentityUser> userManager, ITokenService tokenService, IValidator<RegisterDto> createValidator)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _createValidator = createValidator;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<ActionResult> Register(RegisterDto dto)
    {
        // 1. Validación de FluentValidation
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return MapFluentErrors(validationResult);
        }

        var user = new IdentityUser { UserName = dto.UserName, Email = dto.Email };
        var result = await _userManager.CreateAsync(user, dto.Password);

        // 2. Validación de Identity (Mapeamos a ValidationProblem)
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                // Mapeamos códigos de Identity a nombres de propiedades del DTO
                var key = error.Code.Contains("Email") || error.Code.Contains("UserName") ? "Email" :
                        error.Code.Contains("Password") ? "Password" : "General";
                
                ModelState.AddModelError(key, error.Description);
            }
            return ValidationProblem(ModelState);
        }

        return Created();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return Unauthorized("Credenciales inválidas");

        var token = _tokenService.CreateToken(user);
        return Ok(
            new AuthResponseDto(
                token, 
                user.Id,
                user.UserName!,
                user.Email!, 
                DateTime.UtcNow.AddMinutes(60))
        );
    }
    
    private ActionResult MapFluentErrors(FluentValidation.Results.ValidationResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
        return ValidationProblem(ModelState);
    }
}

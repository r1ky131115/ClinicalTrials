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
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded) 
            return BadRequest(result.Errors);
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
                user.Email!, 
                DateTime.UtcNow.AddMinutes(60))
        );
    }
}

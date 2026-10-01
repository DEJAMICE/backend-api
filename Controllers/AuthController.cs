using Microsoft.AspNetCore.Mvc;
using SafeSignal.Api.DTOs.Auth;
using SafeSignal.Api.Services;
using SafeSignal.Api.Services.Exceptions;

namespace SafeSignal.Api.Controllers;

/// <summary>
/// Controlador RESTful de Autenticación: registro e inicio de sesión con token JWT.
/// Responsable: Persona 2 - Mateo Paolo Salazar Miranda
/// </summary>
[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registra un nuevo usuario y devuelve su token JWT.
    /// </summary>
    /// <param name="request">Nombre, correo, teléfono, contraseña y perfil de uso.</param>
    /// <returns>Token de acceso y datos del usuario creado.</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            return Created("/api/v1/users/me", response);
        }
        catch (DuplicateResourceException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Inicia sesión con correo y contraseña y devuelve un token JWT.
    /// </summary>
    /// <param name="request">Correo y contraseña.</param>
    /// <returns>Token de acceso (Bearer) y datos del usuario.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        if (response == null)
        {
            return Unauthorized(new { message = "Correo o contraseña incorrectos." });
        }

        return Ok(response);
    }
}

using System.ComponentModel.DataAnnotations;

namespace SafeSignal.Api.DTOs.Auth;

/// <summary>
/// DTO de Solicitud: Inicio de sesión con correo y contraseña.
/// </summary>
public class LoginRequest
{
    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Correo electrónico inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    public string Password { get; set; } = string.Empty;
}

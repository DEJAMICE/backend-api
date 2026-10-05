using System.ComponentModel.DataAnnotations;

namespace SafeSignal.Api.DTOs.Users;

/// <summary>
/// DTO de Solicitud: Cambio de contraseña del usuario autenticado.
/// </summary>
public class ChangePasswordRequest
{
    [Required(ErrorMessage = "La contraseña actual es obligatoria")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La nueva contraseña debe tener al menos 8 caracteres")]
    public string NewPassword { get; set; } = string.Empty;
}

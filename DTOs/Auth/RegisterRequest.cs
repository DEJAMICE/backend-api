using System.ComponentModel.DataAnnotations;
using SafeSignal.Api.Domain.Enums;

namespace SafeSignal.Api.DTOs.Auth;

/// <summary>
/// DTO de Solicitud: Registro de un nuevo usuario.
/// </summary>
public class RegisterRequest
{
    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 120 caracteres")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Correo electrónico inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "Teléfono inválido (7 a 15 dígitos, prefijo + opcional)")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    public string Password { get; set; } = string.Empty;

    /// <summary>Perfil de uso: Standard, Student o NightWorker.</summary>
    public UserProfile ProfileType { get; set; } = UserProfile.Standard;
}

using System.ComponentModel.DataAnnotations;
using SafeSignal.Api.Domain.Enums;

namespace SafeSignal.Api.DTOs.Users;

/// <summary>
/// DTO de Solicitud: Actualización de los datos del perfil.
/// </summary>
public class UpdateUserRequest
{
    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 120 caracteres")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "Teléfono inválido (7 a 15 dígitos, prefijo + opcional)")]
    public string PhoneNumber { get; set; } = string.Empty;

    public UserProfile ProfileType { get; set; } = UserProfile.Standard;
}

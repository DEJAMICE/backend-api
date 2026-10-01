using System.ComponentModel.DataAnnotations;
using SafeSignal.Api.Domain.Enums;

namespace SafeSignal.Api.DTOs.Contacts;

/// <summary>
/// DTO de Solicitud: Alta de un contacto de confianza.
/// </summary>
public class CreateContactRequest
{
    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 120 caracteres")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "La relación es obligatoria")]
    [StringLength(50, ErrorMessage = "La relación admite hasta 50 caracteres")]
    public string Relationship { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "Teléfono inválido (7 a 15 dígitos, prefijo + opcional)")]
    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Correo electrónico inválido")]
    public string? Email { get; set; }

    /// <summary>Primary (prioritario), Secondary o EmergencyOnly.</summary>
    public AccessLevel AccessLevel { get; set; } = AccessLevel.Secondary;
}

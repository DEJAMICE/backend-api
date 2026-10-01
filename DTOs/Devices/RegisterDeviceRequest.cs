using System.ComponentModel.DataAnnotations;
using SafeSignal.Api.Domain.Enums;

namespace SafeSignal.Api.DTOs.Devices;

/// <summary>
/// DTO de Solicitud: Vincular un nuevo dispositivo IoT a la cuenta del usuario.
/// </summary>
public class RegisterDeviceRequest
{
    [Required(ErrorMessage = "El nombre del dispositivo es obligatorio")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 60 caracteres")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código del dispositivo es obligatorio")]
    [StringLength(40, MinimumLength = 4, ErrorMessage = "El código debe tener entre 4 y 40 caracteres")]
    public string DeviceCode { get; set; } = string.Empty;

    /// <summary>PanicButton, SmartWatch o TrackerTag.</summary>
    public DeviceType DeviceType { get; set; } = DeviceType.PanicButton;

    [Required(ErrorMessage = "La dirección MAC es obligatoria")]
    [RegularExpression(@"^([0-9A-Fa-f]{2}[:-]){5}[0-9A-Fa-f]{2}$", ErrorMessage = "MAC inválida. Formato esperado: AA:BB:CC:DD:EE:FF")]
    public string MacAddress { get; set; } = string.Empty;
}

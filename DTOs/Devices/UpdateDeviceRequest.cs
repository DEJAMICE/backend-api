using System.ComponentModel.DataAnnotations;
using SafeSignal.Api.Domain.Enums;

namespace SafeSignal.Api.DTOs.Devices;

/// <summary>
/// DTO de Solicitud: Edición del nombre y tipo de un dispositivo vinculado.
/// </summary>
public class UpdateDeviceRequest
{
    [Required(ErrorMessage = "El nombre del dispositivo es obligatorio")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 60 caracteres")]
    public string Name { get; set; } = string.Empty;

    public DeviceType DeviceType { get; set; } = DeviceType.PanicButton;
}

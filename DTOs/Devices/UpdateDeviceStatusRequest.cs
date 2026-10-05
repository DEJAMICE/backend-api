using System.ComponentModel.DataAnnotations;

namespace SafeSignal.Api.DTOs.Devices;

/// <summary>
/// DTO de Solicitud: Reporte de estado del dispositivo (batería y conectividad), enviado por la app móvil / el propio dispositivo.
/// </summary>
public class UpdateDeviceStatusRequest
{
    [Range(0, 100, ErrorMessage = "La batería debe estar entre 0 y 100")]
    public int BatteryLevel { get; set; } = 100;

    public bool IsConnected { get; set; }
}

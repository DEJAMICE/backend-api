using System.ComponentModel.DataAnnotations;
using SafeSignal.Api.Domain.Enums;

namespace SafeSignal.Api.DTOs.Alerts;

/// <summary>
/// DTO de Solicitud: Parámetros requeridos para emitir una alerta de emergencia SOS.
/// </summary>
public class CreateAlertRequest
{
    [Required(ErrorMessage = "La latitud es obligatoria")]
    [Range(-90.0, 90.0, ErrorMessage = "Latitud fuera de rango")]
    public double Latitude { get; set; }

    [Required(ErrorMessage = "La longitud es obligatoria")]
    [Range(-180.0, 180.0, ErrorMessage = "Longitud fuera de rango")]
    public double Longitude { get; set; }

    public string? LocationAddress { get; set; }

    public AlertType Type { get; set; } = AlertType.PanicButton;

    public AlertSeverity Severity { get; set; } = AlertSeverity.Critical;

    public string? DeviceId { get; set; }

    public string? DeviceName { get; set; }

    public int BatteryLevel { get; set; } = 100;

    public string? Notes { get; set; }
}

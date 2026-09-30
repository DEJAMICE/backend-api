using SafeSignal.Api.Domain.Enums;

namespace SafeSignal.Api.DTOs.Alerts;

/// <summary>
/// DTO de Respuesta: Estructura de salida devuelta por los endpoints de alertas SOS.
/// </summary>
public class AlertResponse
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string LocationAddress { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string? DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public int BatteryLevel { get; set; }
    public int ContactsNotifiedCount { get; set; }
    public bool PoliceNotified { get; set; }
    public string? Notes { get; set; }
    public string? ResolutionNotes { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}

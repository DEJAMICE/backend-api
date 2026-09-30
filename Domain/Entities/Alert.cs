using SafeSignal.Api.Domain.Enums;

namespace SafeSignal.Api.Domain.Entities;

/// <summary>
/// Entidad del Dominio: Representa una alerta de auxilio o emergencia emitida en el sistema.
/// </summary>
public class Alert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string LocationAddress { get; set; } = string.Empty;
    public AlertType Type { get; set; } = AlertType.PanicButton;
    public AlertStatus Status { get; set; } = AlertStatus.Active;
    public AlertSeverity Severity { get; set; } = AlertSeverity.Critical;
    public string? DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public int BatteryLevel { get; set; } = 100;
    public int ContactsNotifiedCount { get; set; }
    public bool PoliceNotified { get; set; }
    public string? Notes { get; set; }
    public string? ResolutionNotes { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}

using SafeSignal.Api.Domain.Enums;

namespace SafeSignal.Api.Domain.Entities;

/// <summary>
/// Entidad del Dominio: Representa una sesión de monitoreo de ruta segura.
/// </summary>
public class TrackingRoute
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public string OriginAddress { get; set; } = string.Empty;
    public string DestinationAddress { get; set; } = string.Empty;
    public RouteStatus Status { get; set; } = RouteStatus.InProgress;
    public int EstimatedDurationMinutes { get; set; }
    public bool IsDeviationDetected { get; set; }
    public bool ShareWithContacts { get; set; } = true;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public List<TrackingPoint> Waypoints { get; set; } = new();
}

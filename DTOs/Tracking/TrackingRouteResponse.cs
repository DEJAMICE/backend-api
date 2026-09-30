namespace SafeSignal.Api.DTOs.Tracking;

/// <summary>
/// DTO de Respuesta: Información consolidada de la sesión de seguimiento en curso o histórica.
/// </summary>
public class TrackingRouteResponse
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string OriginAddress { get; set; } = string.Empty;
    public string DestinationAddress { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int EstimatedDurationMinutes { get; set; }
    public bool IsDeviationDetected { get; set; }
    public bool ShareWithContacts { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<TrackingPointResponse> Waypoints { get; set; } = new();
}

namespace SafeSignal.Api.Domain.Entities;

/// <summary>
/// Entidad del Dominio: Representa una coordenada GPS registrada durante un trayecto monitoreado.
/// </summary>
public class TrackingPoint
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RouteId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Speed { get; set; }
    public double Heading { get; set; }
    public bool IsDeviation { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

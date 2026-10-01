namespace SafeSignal.Api.DTOs.Tracking;

/// <summary>
/// DTO de Respuesta: Punto geolocalizado en el mapa.
/// </summary>
public class TrackingPointResponse
{
    public Guid Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Speed { get; set; }
    public double Heading { get; set; }
    public bool IsDeviation { get; set; }
    public DateTime Timestamp { get; set; }
}

namespace SafeSignal.Api.Domain.Entities;

/// <summary>
/// Entidad del Dominio: Reporte comunitario de incidente o zona de riesgo urbano.
/// </summary>
public class CommunityReport
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Category { get; set; } = "Robo"; // "Robo", "Zona Oscura", "Acoso", "Sospechoso", "Otro"
    public string? SecondaryCategory { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int ConfirmedCount { get; set; } = 1;
    public int RefutedCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

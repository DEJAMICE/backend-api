namespace SafeSignal.Api.DTOs.Reports;

/// <summary>
/// DTO de respuesta para un reporte comunitario.
/// </summary>
public class ReportResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? SecondaryCategory { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int ConfirmedCount { get; set; }
    public int RefutedCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool ValidatedByMe { get; set; }
}

/// <summary>
/// DTO de solicitud para publicar un reporte de incidente.
/// </summary>
public class CreateReportRequest
{
    public string Category { get; set; } = "Robo";
    public string? SecondaryCategory { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

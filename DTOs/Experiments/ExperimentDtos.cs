using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace SafeSignal.Api.DTOs.Experiments;

/// <summary>
/// Solicitud para registrar un evento de experimento en el servidor (sin datos personales).
/// </summary>
public class TrackExperimentEventRequest
{
    [Required(ErrorMessage = "El nombre del evento es obligatorio.")]
    [RegularExpression(@"^[a-z][a-z0-9_]*$", ErrorMessage = "El nombre del evento debe usar snake_case en minúsculas.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(32)]
    public string? ExperimentId { get; set; }

    [RegularExpression(@"^(A|B)$", ErrorMessage = "La variante debe ser 'A' o 'B'.")]
    public string? Variant { get; set; }

    [RegularExpression(@"^(landing|web|android|ios|backend)$", ErrorMessage = "Plataforma no válida.")]
    public string Platform { get; set; } = "web";

    [StringLength(32)]
    public string AppVersion { get; set; } = "1.0.0";

    [StringLength(128)]
    public string? ClientId { get; set; }

    public Dictionary<string, object>? Params { get; set; }
}

/// <summary>
/// Respuesta resumida del evento registrado (seudonimizado).
/// </summary>
public class ExperimentEventResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UserPseudoId { get; set; } = string.Empty;
    public string? ExperimentId { get; set; }
    public string? Variant { get; set; }
    public string Platform { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
}

/// <summary>
/// Conteo agregado por experimento, variante y evento para análisis estadístico.
/// </summary>
public class ExperimentMetricBucket
{
    public string ExperimentId { get; set; } = string.Empty;
    public string Variant { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public int TotalEvents { get; set; }
    public int UniqueParticipants { get; set; }
}

/// <summary>
/// Resumen consolidado de eventos registrados por experimento.
/// </summary>
public class ExperimentSummaryResponse
{
    public int TotalEvents { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public IReadOnlyList<ExperimentMetricBucket> Buckets { get; set; } = Array.Empty<ExperimentMetricBucket>();
}

namespace SafeSignal.Api.Domain.Entities;

/// <summary>
/// Entidad de Dominio: Evento de experimentación registrado en servidor sin PII (Ley N.º 29733).
/// </summary>
public class ExperimentEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
    public string UserPseudoId { get; init; } = string.Empty;
    public string? ExperimentId { get; init; }
    public string? Variant { get; init; }
    public string Platform { get; init; } = "web";
    public string AppVersion { get; init; } = "1.0.0";
    public string PayloadJson { get; init; } = "{}";
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}

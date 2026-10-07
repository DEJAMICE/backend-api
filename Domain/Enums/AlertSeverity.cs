namespace SafeSignal.Api.Domain.Enums;

/// <summary>
/// Nivel de gravedad o severidad de una alerta.
/// </summary>
public enum AlertSeverity
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3,
    CRITICAL = Critical,
    HIGH = High,
    MEDIUM = Medium,
    LOW = Low
}

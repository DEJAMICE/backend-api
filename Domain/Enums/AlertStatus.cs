namespace SafeSignal.Api.Domain.Enums;

/// <summary>
/// Representa el ciclo de vida de una alerta de emergencia SOS en SafeSignal.
/// </summary>
public enum AlertStatus
{
    Active = 0,
    Resolved = 1,
    Cancelled = 2,
    FalseAlarm = 3
}

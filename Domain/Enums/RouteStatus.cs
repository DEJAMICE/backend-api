namespace SafeSignal.Api.Domain.Enums;

/// <summary>
/// Estado del monitoreo en vivo de una ruta de traslado.
/// </summary>
public enum RouteStatus
{
    InProgress = 0,
    Completed = 1,
    Cancelled = 2,
    Interrupted = 3
}

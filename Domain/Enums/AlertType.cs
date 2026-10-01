namespace SafeSignal.Api.Domain.Enums;

/// <summary>
/// Tipos de alerta admitidos por el sistema SafeSignal.
/// </summary>
public enum AlertType
{
    PanicButton = 0,
    SilentAlert = 1,
    MedicalEmergency = 2,
    Harassment = 3,
    RouteDeviation = 4
}

namespace SafeSignal.Api.Domain.Enums;

/// <summary>
/// Nivel de prioridad de un contacto de confianza.
/// Primary = Contacto prioritario; Secondary = contacto regular; EmergencyOnly = solo se notifica en SOS.
/// </summary>
public enum AccessLevel
{
    Primary = 0,
    Secondary = 1,
    EmergencyOnly = 2
}

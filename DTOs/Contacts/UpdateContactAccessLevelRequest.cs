using SafeSignal.Api.Domain.Enums;

namespace SafeSignal.Api.DTOs.Contacts;

/// <summary>
/// DTO de Solicitud: Cambio rápido del nivel de prioridad de un contacto.
/// </summary>
public class UpdateContactAccessLevelRequest
{
    public AccessLevel AccessLevel { get; set; } = AccessLevel.Secondary;
}

using System.ComponentModel.DataAnnotations;

namespace SafeSignal.Api.DTOs.Tracking;

/// <summary>
/// DTO de Solicitud: Iniciar un nuevo recorrido seguro con telemetría en vivo.
/// </summary>
public class StartTrackingRequest
{
    [Required(ErrorMessage = "La dirección de origen es requerida")]
    public string OriginAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección de destino es requerida")]
    public string DestinationAddress { get; set; } = string.Empty;

    [Range(1, 600, ErrorMessage = "La duración estimada debe estar entre 1 y 600 minutos")]
    public int EstimatedDurationMinutes { get; set; } = 30;

    public bool ShareWithContacts { get; set; } = true;
}

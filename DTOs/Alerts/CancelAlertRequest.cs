using System.ComponentModel.DataAnnotations;

namespace SafeSignal.Api.DTOs.Alerts;

/// <summary>
/// DTO de Solicitud: Parámetros para cancelar una alerta emitida por error.
/// </summary>
public class CancelAlertRequest
{
    [Required(ErrorMessage = "El motivo de cancelación es obligatorio")]
    public string Reason { get; set; } = "Falsa alarma / Activación accidental";
}

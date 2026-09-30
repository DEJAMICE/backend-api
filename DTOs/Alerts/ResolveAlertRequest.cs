using System.ComponentModel.DataAnnotations;

namespace SafeSignal.Api.DTOs.Alerts;

/// <summary>
/// DTO de Solicitud: Parámetros para dar por resuelta una alerta activa.
/// </summary>
public class ResolveAlertRequest
{
    [Required(ErrorMessage = "Las notas de resolución son requeridas")]
    public string ResolutionNotes { get; set; } = string.Empty;
}

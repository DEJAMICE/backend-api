using System.ComponentModel.DataAnnotations;

namespace SafeSignal.Api.DTOs.Tracking;

/// <summary>
/// DTO de Solicitud: Enviar una coordenada de localización GPS en tiempo real.
/// </summary>
public class CreateTrackingPointRequest
{
    [Required(ErrorMessage = "El ID de la ruta es obligatorio")]
    public Guid RouteId { get; set; }

    [Required(ErrorMessage = "La latitud es obligatoria")]
    [Range(-90.0, 90.0, ErrorMessage = "Latitud fuera de rango")]
    public double Latitude { get; set; }

    [Required(ErrorMessage = "La longitud es obligatoria")]
    [Range(-180.0, 180.0, ErrorMessage = "Longitud fuera de rango")]
    public double Longitude { get; set; }

    public double Speed { get; set; }

    public double Heading { get; set; }
}

using Microsoft.AspNetCore.Mvc;
using SafeSignal.Api.DTOs.Tracking;
using SafeSignal.Api.Services;

namespace SafeSignal.Api.Controllers;

/// <summary>
/// Controlador RESTful para la telemetría y seguimiento en tiempo real de trayectos seguros.
/// Responsable: Persona 3 - Mathias Andree Cárdenas Huamán
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class TrackingController : ControllerBase
{
    private readonly ITrackingService _trackingService;

    public TrackingController(ITrackingService trackingService)
    {
        _trackingService = trackingService;
    }

    /// <summary>
    /// Inicia una nueva sesión de seguimiento de ruta segura.
    /// </summary>
    [HttpPost("start")]
    [ProducesResponseType(typeof(TrackingRouteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartTracking([FromBody] StartTrackingRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = User.Identity?.Name ?? "usr_001";
        var route = await _trackingService.StartTrackingAsync(request, userId);
        return CreatedAtAction(nameof(GetLiveRoute), new { routeId = route.Id }, route);
    }

    /// <summary>
    /// Registra un nuevo punto de geolocalización (waypoint) en una ruta activa.
    /// Evalúa automáticamente anomalías o desviaciones respecto al trayecto.
    /// </summary>
    [HttpPost("points")]
    [ProducesResponseType(typeof(TrackingPointResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddTrackingPoint([FromBody] CreateTrackingPointRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var point = await _trackingService.AddTrackingPointAsync(request);
        if (point == null)
        {
            return NotFound(new { message = $"No se encontró la ruta activa con ID '{request.RouteId}'." });
        }

        return Ok(point);
    }

    /// <summary>
    /// Consulta el estado actual y los waypoints recorridos de una ruta en vivo.
    /// </summary>
    [HttpGet("{routeId:guid}/live")]
    [ProducesResponseType(typeof(TrackingRouteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLiveRoute(Guid routeId)
    {
        var route = await _trackingService.GetLiveRouteAsync(routeId);
        if (route == null)
        {
            return NotFound(new { message = $"Ruta con ID '{routeId}' no encontrada." });
        }

        return Ok(route);
    }

    /// <summary>
    /// Finaliza la sesión de seguimiento de ruta indicando que el usuario llegó a su destino seguro.
    /// </summary>
    [HttpPost("{routeId:guid}/stop")]
    [ProducesResponseType(typeof(TrackingRouteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StopTracking(Guid routeId)
    {
        var route = await _trackingService.StopTrackingAsync(routeId);
        if (route == null)
        {
            return NotFound(new { message = $"No se encontró la ruta con ID '{routeId}' para finalizar." });
        }

        return Ok(route);
    }

    /// <summary>
    /// Obtiene el historial de rutas seguras completadas.
    /// </summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(IEnumerable<TrackingRouteResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory([FromQuery] string? userId)
    {
        var history = await _trackingService.GetHistoryAsync(userId);
        return Ok(history);
    }
}

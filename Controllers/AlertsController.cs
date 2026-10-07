using Microsoft.AspNetCore.Mvc;
using SafeSignal.Api.DTOs.Alerts;
using SafeSignal.Api.Services;

namespace SafeSignal.Api.Controllers;

/// <summary>
/// Controlador RESTful para la gestión y despacho de Alertas SOS de Emergencia.
/// Responsable: Persona 3 - Mathias Andree Cárdenas Huamán
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AlertsController : ControllerBase
{
    private readonly IAlertService _alertService;

    public AlertsController(IAlertService alertService)
    {
        _alertService = alertService;
    }

    /// <summary>
    /// Emite una nueva alerta de auxilio SOS geolocalizada.
    /// </summary>
    /// <param name="request">Datos geográficos, tipo de emergencia y dispositivo emisor.</param>
    /// <returns>La alerta creada con estado Activo y detalles de notificación.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AlertResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EmitAlert([FromBody] CreateAlertRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Obtener usuario (autenticado por JWT o valores predeterminados)
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value
                     ?? User.Identity?.Name
                     ?? "usr_001";
        var userName = User.FindFirst("name")?.Value
                       ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
                       ?? "Usuario Demo SafeSignal";

        var createdAlert = await _alertService.EmitAlertAsync(request, userId, userName);
        return CreatedAtAction(nameof(GetAlertById), new { id = createdAlert.Id }, createdAlert);
    }

    /// <summary>
    /// Obtiene la lista de alertas SOS que se encuentran actualmente activas en tiempo real.
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<AlertResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveAlerts()
    {
        var alerts = await _alertService.GetActiveAlertsAsync();
        return Ok(alerts);
    }

    /// <summary>
    /// Obtiene los detalles de una alerta específica mediante su identificador único.
    /// </summary>
    /// <param name="id">GUID de la alerta.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AlertResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAlertById(Guid id)
    {
        var alert = await _alertService.GetAlertByIdAsync(id);
        if (alert == null)
        {
            return NotFound(new { message = $"Alerta con ID '{id}' no encontrada." });
        }

        return Ok(alert);
    }

    /// <summary>
    /// Obtiene el histórico de alertas emitidas (filtrable opcionalmente por usuario).
    /// </summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(IEnumerable<AlertResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAlertHistory([FromQuery] string? userId)
    {
        var history = await _alertService.GetAlertHistoryAsync(userId);
        return Ok(history);
    }

    /// <summary>
    /// Marca una alerta activa como resuelta tras recibir auxilio o verificar la seguridad del usuario.
    /// </summary>
    [HttpPut("{id:guid}/resolve")]
    [ProducesResponseType(typeof(AlertResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResolveAlert(Guid id, [FromBody] ResolveAlertRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resolved = await _alertService.ResolveAlertAsync(id, request);
        if (resolved == null)
        {
            return NotFound(new { message = $"No se encontró la alerta con ID '{id}' para resolver." });
        }

        return Ok(resolved);
    }

    /// <summary>
    /// Cancela una alerta activada accidentalmente o por falsa alarma.
    /// </summary>
    [HttpPut("{id:guid}/cancel")]
    [ProducesResponseType(typeof(AlertResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelAlert(Guid id, [FromBody] CancelAlertRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var cancelled = await _alertService.CancelAlertAsync(id, request);
        if (cancelled == null)
        {
            return NotFound(new { message = $"No se encontró la alerta con ID '{id}' para cancelar." });
        }

        return Ok(cancelled);
    }
}

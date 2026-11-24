using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeSignal.Api.DTOs.Experiments;
using SafeSignal.Api.Services;

namespace SafeSignal.Api.Controllers;

/// <summary>
/// Registro y consulta agregada de eventos para análisis experimental (TS02).
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/v1/experiments")]
[Produces("application/json")]
public class ExperimentsController : ControllerBase
{
    private readonly IExperimentService _experimentService;

    public ExperimentsController(IExperimentService experimentService)
    {
        _experimentService = experimentService;
    }

    /// <summary>
    /// Registra un evento de experimento seudonimizado (sin datos personales).
    /// </summary>
    [HttpPost("events")]
    [ProducesResponseType(typeof(ExperimentEventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TrackEvent([FromBody] TrackExperimentEventRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var rawId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub")
                    ?? Request.Headers["X-Client-Id"].FirstOrDefault()
                    ?? request.ClientId
                    ?? "anonymous";

        var saved = await _experimentService.TrackEventAsync(request, rawId);
        return StatusCode(StatusCodes.Status201Created, saved);
    }

    /// <summary>
    /// Retorna el resumen agregado de eventos por experimento y variante para análisis estadístico.
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ExperimentSummaryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary([FromQuery] string? experimentId)
    {
        var summary = await _experimentService.GetSummaryAsync(experimentId);
        return Ok(summary);
    }

    /// <summary>
    /// Lista los últimos eventos seudonimizados registrados (para verificación en QA/Debug).
    /// </summary>
    [HttpGet("events")]
    [ProducesResponseType(typeof(IReadOnlyList<ExperimentEventResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentEvents([FromQuery] int limit = 50)
    {
        var events = await _experimentService.GetRecentEventsAsync(limit);
        return Ok(events);
    }
}

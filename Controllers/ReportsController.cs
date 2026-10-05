using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeSignal.Api.Controllers.Extensions;
using SafeSignal.Api.DTOs.Reports;
using SafeSignal.Api.Services;

namespace SafeSignal.Api.Controllers;

/// <summary>
/// Controlador RESTful para la publicación, consulta y validación de Reportes Comunitarios de Seguridad.
/// </summary>
[ApiController]
[Route("api/v1/reports")]
[Produces("application/json")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly IUserService _userService;

    public ReportsController(IReportService reportService, IUserService userService)
    {
        _reportService = reportService;
        _userService = userService;
    }

    /// <summary>
    /// Obtiene todos los reportes de incidentes comunitarios ordenados por fecha.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReportResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllReports()
    {
        var currentUserId = User.GetUserId();
        var reports = await _reportService.GetAllAsync(currentUserId);
        return Ok(reports);
    }

    /// <summary>
    /// Obtiene los reportes emitidos exclusivamente por el usuario autenticado.
    /// </summary>
    [Authorize]
    [HttpGet("my")]
    [ProducesResponseType(typeof(IEnumerable<ReportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyReports()
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var reports = await _reportService.GetByUserIdAsync(userId);
        return Ok(reports);
    }

    /// <summary>
    /// Publica un nuevo reporte de incidente urbano en la red comunitaria.
    /// </summary>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ReportResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateReport([FromBody] CreateReportRequest request)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        if (string.IsNullOrWhiteSpace(request.Address) || string.IsNullOrWhiteSpace(request.Description))
        {
            return BadRequest(new { message = "La dirección y la descripción del reporte son obligatorias." });
        }

        var profile = await _userService.GetProfileAsync(userId);
        var userName = profile?.FullName ?? "Ciudadano";

        var created = await _reportService.CreateAsync(userId, userName, request);
        return Created($"/api/v1/reports/{created.Id}", created);
    }

    /// <summary>
    /// Valida / confirma la veracidad de un reporte por parte de un vecino.
    /// </summary>
    [Authorize]
    [HttpPost("{id:guid}/validate")]
    [ProducesResponseType(typeof(ReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ValidateReport(Guid id)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var updated = await _reportService.ValidateAsync(id, userId, isConfirm: true);
        return updated == null ? NotFound(new { message = "Reporte no encontrado." }) : Ok(updated);
    }

    /// <summary>
    /// Marca un reporte como dudoso o no confirmado.
    /// </summary>
    [Authorize]
    [HttpPost("{id:guid}/refute")]
    [ProducesResponseType(typeof(ReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RefuteReport(Guid id)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var updated = await _reportService.ValidateAsync(id, userId, isConfirm: false);
        return updated == null ? NotFound(new { message = "Reporte no encontrado." }) : Ok(updated);
    }

    /// <summary>
    /// Elimina un reporte publicado por el usuario.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReport(Guid id)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var deleted = await _reportService.DeleteAsync(id, userId);
        return deleted ? NoContent() : NotFound(new { message = "Reporte no encontrado o no tiene permisos para eliminarlo." });
    }
}

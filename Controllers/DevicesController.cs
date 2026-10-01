using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeSignal.Api.Controllers.Extensions;
using SafeSignal.Api.DTOs.Devices;
using SafeSignal.Api.Services;
using SafeSignal.Api.Services.Exceptions;

namespace SafeSignal.Api.Controllers;

/// <summary>
/// Controlador RESTful de Dispositivos IoT (botón de pánico, reloj inteligente, tracker). Requiere token JWT; cada usuario solo ve y modifica sus propios dispositivos.
/// Responsable: Persona 2 - Mateo Paolo Salazar Miranda
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/devices")]
[Produces("application/json")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DevicesController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    /// <summary>
    /// Vincula un nuevo dispositivo IoT a la cuenta del usuario.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterDeviceRequest request)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        try
        {
            var created = await _deviceService.RegisterAsync(userId, request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (DuplicateResourceException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lista los dispositivos IoT vinculados al usuario.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DeviceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        return Ok(await _deviceService.GetAllAsync(userId));
    }

    /// <summary>
    /// Obtiene el detalle de un dispositivo (batería, conectividad, última conexión).
    /// </summary>
    /// <param name="id">GUID del dispositivo.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var device = await _deviceService.GetByIdAsync(userId, id);
        return device == null ? NotFound(new { message = $"Dispositivo con ID '{id}' no encontrado." }) : Ok(device);
    }

    /// <summary>
    /// Edita el nombre y tipo de un dispositivo vinculado.
    /// </summary>
    /// <param name="id">GUID del dispositivo.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDeviceRequest request)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var updated = await _deviceService.UpdateAsync(userId, id, request);
        return updated == null ? NotFound(new { message = $"Dispositivo con ID '{id}' no encontrado." }) : Ok(updated);
    }

    /// <summary>
    /// Reporta el estado del dispositivo (nivel de batería y conectividad). Lo consume la app móvil.
    /// </summary>
    /// <param name="id">GUID del dispositivo.</param>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateDeviceStatusRequest request)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var updated = await _deviceService.UpdateStatusAsync(userId, id, request);
        return updated == null ? NotFound(new { message = $"Dispositivo con ID '{id}' no encontrado." }) : Ok(updated);
    }

    /// <summary>
    /// Desvincula un dispositivo de la cuenta del usuario.
    /// </summary>
    /// <param name="id">GUID del dispositivo.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unlink(Guid id)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var deleted = await _deviceService.DeleteAsync(userId, id);
        return deleted ? NoContent() : NotFound(new { message = $"Dispositivo con ID '{id}' no encontrado." });
    }
}

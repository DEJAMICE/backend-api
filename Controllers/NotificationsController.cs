using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeSignal.Api.Controllers.Extensions;
using SafeSignal.Api.DTOs.Notifications;
using SafeSignal.Api.Services;

namespace SafeSignal.Api.Controllers;

/// <summary>
/// Controlador RESTful para la gestión de notificaciones personales y alertas ciudadanas.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/notifications")]
[Produces("application/json")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Obtiene el historial de notificaciones del usuario autenticado.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NotificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyNotifications()
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var notifs = await _notificationService.GetByUserIdAsync(userId);
        return Ok(notifs);
    }

    /// <summary>
    /// Obtiene el conteo de notificaciones no leídas para actualizar la campanita del topbar.
    /// </summary>
    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUnreadCount()
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var count = await _notificationService.GetUnreadCountAsync(userId);
        return Ok(new { unreadCount = count });
    }

    /// <summary>
    /// Marca una notificación específica como leída.
    /// </summary>
    [HttpPut("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var success = await _notificationService.MarkAsReadAsync(id, userId);
        return success ? Ok(new { message = "Notificación marcada como leída." }) : NotFound(new { message = "Notificación no encontrada." });
    }

    /// <summary>
    /// Marca todas las notificaciones pendientes del usuario como leídas.
    /// </summary>
    [HttpPut("read-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var updatedCount = await _notificationService.MarkAllAsReadAsync(userId);
        return Ok(new { updatedCount, message = "Todas las notificaciones fueron marcadas como leídas." });
    }

    /// <summary>
    /// Registra una nueva notificación (para alertas de ruta o de auxilio).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(NotificationResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationRequest request)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var created = await _notificationService.CreateAsync(userId, request);
        return Created($"/api/v1/notifications/{created.Id}", created);
    }

    /// <summary>
    /// Elimina una notificación.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteNotification(Guid id)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var deleted = await _notificationService.DeleteAsync(id, userId);
        return deleted ? NoContent() : NotFound(new { message = "Notificación no encontrada." });
    }
}

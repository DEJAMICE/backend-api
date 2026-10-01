using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeSignal.Api.Controllers.Extensions;
using SafeSignal.Api.DTOs.Users;
using SafeSignal.Api.Services;

namespace SafeSignal.Api.Controllers;

/// <summary>
/// Controlador RESTful de Usuarios y Perfiles. Todos los endpoints requieren token JWT y operan sobre el usuario autenticado.
/// Responsable: Persona 2 - Mateo Paolo Salazar Miranda
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/users")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IContactService _contactService;
    private readonly IDeviceService _deviceService;

    public UsersController(IUserService userService, IContactService contactService, IDeviceService deviceService)
    {
        _userService = userService;
        _contactService = contactService;
        _deviceService = deviceService;
    }

    /// <summary>
    /// Obtiene el perfil del usuario autenticado.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfile()
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var profile = await _userService.GetProfileAsync(userId);
        return profile == null ? NotFound(new { message = "Usuario no encontrado." }) : Ok(profile);
    }

    /// <summary>
    /// Actualiza el nombre, teléfono y perfil de uso del usuario autenticado.
    /// </summary>
    [HttpPut("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserRequest request)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var updated = await _userService.UpdateProfileAsync(userId, request);
        return updated == null ? NotFound(new { message = "Usuario no encontrado." }) : Ok(updated);
    }

    /// <summary>
    /// Cambia la contraseña del usuario autenticado.
    /// </summary>
    [HttpPut("me/password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var result = await _userService.ChangePasswordAsync(userId, request);
        return result switch
        {
            ChangePasswordResult.Success => NoContent(),
            ChangePasswordResult.InvalidCurrentPassword => BadRequest(new { message = "La contraseña actual es incorrecta." }),
            _ => NotFound(new { message = "Usuario no encontrado." })
        };
    }

    /// <summary>
    /// Cambia el plan de suscripción (Free / Premium) del usuario autenticado.
    /// </summary>
    [HttpPut("me/subscription")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSubscription([FromBody] UpdateSubscriptionRequest request)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var updated = await _userService.UpdateSubscriptionAsync(userId, request);
        return updated == null ? NotFound(new { message = "Usuario no encontrado." }) : Ok(updated);
    }

    /// <summary>
    /// Elimina la cuenta del usuario autenticado junto con sus contactos de confianza y dispositivos vinculados.
    /// </summary>
    [HttpDelete("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMyAccount()
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var deleted = await _userService.DeleteAsync(userId);
        if (!deleted) return NotFound(new { message = "Usuario no encontrado." });

        await _contactService.DeleteAllByUserAsync(userId);
        await _deviceService.DeleteAllByUserAsync(userId);
        return NoContent();
    }
}

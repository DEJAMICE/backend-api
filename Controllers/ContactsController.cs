using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeSignal.Api.Controllers.Extensions;
using SafeSignal.Api.Domain.Enums;
using SafeSignal.Api.DTOs.Contacts;
using SafeSignal.Api.Services;

namespace SafeSignal.Api.Controllers;

/// <summary>
/// Controlador RESTful de Contactos de Confianza (red de apoyo). Requiere token JWT; cada usuario solo ve y modifica sus propios contactos.
/// Responsable: Persona 2 - Mateo Paolo Salazar Miranda
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/contacts")]
[Produces("application/json")]
public class ContactsController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactsController(IContactService contactService)
    {
        _contactService = contactService;
    }

    /// <summary>
    /// Agrega un contacto de confianza a la red de apoyo del usuario.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateContactRequest request)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var created = await _contactService.CreateAsync(userId, request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Lista los contactos de confianza del usuario, ordenados por prioridad. Filtro opcional por nivel de acceso.
    /// </summary>
    /// <param name="accessLevel">Primary, Secondary o EmergencyOnly (opcional).</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ContactResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll([FromQuery] AccessLevel? accessLevel)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        return Ok(await _contactService.GetAllAsync(userId, accessLevel));
    }

    /// <summary>
    /// Obtiene el detalle de un contacto de confianza.
    /// </summary>
    /// <param name="id">GUID del contacto.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var contact = await _contactService.GetByIdAsync(userId, id);
        return contact == null ? NotFound(new { message = $"Contacto con ID '{id}' no encontrado." }) : Ok(contact);
    }

    /// <summary>
    /// Actualiza todos los datos de un contacto de confianza.
    /// </summary>
    /// <param name="id">GUID del contacto.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContactRequest request)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var updated = await _contactService.UpdateAsync(userId, id, request);
        return updated == null ? NotFound(new { message = $"Contacto con ID '{id}' no encontrado." }) : Ok(updated);
    }

    /// <summary>
    /// Cambia el nivel de prioridad de un contacto (por ejemplo, asignarlo como Contacto prioritario).
    /// </summary>
    /// <param name="id">GUID del contacto.</param>
    [HttpPatch("{id:guid}/access-level")]
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAccessLevel(Guid id, [FromBody] UpdateContactAccessLevelRequest request)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var updated = await _contactService.UpdateAccessLevelAsync(userId, id, request);
        return updated == null ? NotFound(new { message = $"Contacto con ID '{id}' no encontrado." }) : Ok(updated);
    }

    /// <summary>
    /// Elimina un contacto de la red de confianza.
    /// </summary>
    /// <param name="id">GUID del contacto.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (User.GetUserId() is not Guid userId) return Unauthorized();

        var deleted = await _contactService.DeleteAsync(userId, id);
        return deleted ? NoContent() : NotFound(new { message = $"Contacto con ID '{id}' no encontrado." });
    }
}

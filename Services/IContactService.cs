using SafeSignal.Api.Domain.Enums;
using SafeSignal.Api.DTOs.Contacts;

namespace SafeSignal.Api.Services;

/// <summary>
/// Contrato del Servicio de Contactos de Confianza (red de apoyo). Todas las operaciones se limitan al usuario dueño.
/// </summary>
public interface IContactService
{
    Task<ContactResponse> CreateAsync(Guid userId, CreateContactRequest request);
    Task<IEnumerable<ContactResponse>> GetAllAsync(Guid userId, AccessLevel? accessLevel = null);
    Task<ContactResponse?> GetByIdAsync(Guid userId, Guid contactId);
    Task<ContactResponse?> UpdateAsync(Guid userId, Guid contactId, UpdateContactRequest request);
    Task<ContactResponse?> UpdateAccessLevelAsync(Guid userId, Guid contactId, UpdateContactAccessLevelRequest request);
    Task<bool> DeleteAsync(Guid userId, Guid contactId);
    Task DeleteAllByUserAsync(Guid userId);
}

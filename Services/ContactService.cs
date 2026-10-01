using System.Collections.Concurrent;
using SafeSignal.Api.Domain.Entities;
using SafeSignal.Api.Domain.Enums;
using SafeSignal.Api.DTOs.Contacts;

namespace SafeSignal.Api.Services;

/// <summary>
/// Implementación de la lógica de negocio de Contactos de Confianza (almacenamiento en memoria).
/// </summary>
public class ContactService : IContactService
{
    private readonly ConcurrentDictionary<Guid, TrustContact> _contacts = new();

    public ContactService()
    {
        // Datos semilla del usuario demo
        var seed = new[]
        {
            new TrustContact
            {
                Id = Guid.Parse("bbbbbbbb-0000-0000-0000-000000000001"),
                UserId = UserService.DemoUserId,
                FullName = "María Torres",
                Relationship = "Madre",
                PhoneNumber = "+51988777666",
                Email = "maria.torres@example.com",
                AccessLevel = AccessLevel.Primary,
                CreatedAt = DateTime.UtcNow.AddDays(-6)
            },
            new TrustContact
            {
                Id = Guid.Parse("bbbbbbbb-0000-0000-0000-000000000002"),
                UserId = UserService.DemoUserId,
                FullName = "Carlos Ruiz",
                Relationship = "Amigo",
                PhoneNumber = "+51977666555",
                AccessLevel = AccessLevel.Secondary,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            }
        };

        foreach (var contact in seed)
        {
            _contacts.TryAdd(contact.Id, contact);
        }
    }

    public Task<ContactResponse> CreateAsync(Guid userId, CreateContactRequest request)
    {
        var contact = new TrustContact
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = request.FullName.Trim(),
            Relationship = request.Relationship.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            AccessLevel = request.AccessLevel,
            CreatedAt = DateTime.UtcNow
        };

        _contacts.TryAdd(contact.Id, contact);
        return Task.FromResult(MapToResponse(contact));
    }

    public Task<IEnumerable<ContactResponse>> GetAllAsync(Guid userId, AccessLevel? accessLevel = null)
    {
        var query = _contacts.Values.Where(c => c.UserId == userId);
        if (accessLevel.HasValue)
        {
            query = query.Where(c => c.AccessLevel == accessLevel.Value);
        }

        var result = query
            .OrderBy(c => c.AccessLevel)
            .ThenBy(c => c.FullName)
            .Select(MapToResponse)
            .ToList();

        return Task.FromResult<IEnumerable<ContactResponse>>(result);
    }

    public Task<ContactResponse?> GetByIdAsync(Guid userId, Guid contactId)
    {
        var contact = FindOwned(userId, contactId);
        return Task.FromResult(contact == null ? null : MapToResponse(contact));
    }

    public Task<ContactResponse?> UpdateAsync(Guid userId, Guid contactId, UpdateContactRequest request)
    {
        var contact = FindOwned(userId, contactId);
        if (contact == null)
        {
            return Task.FromResult<ContactResponse?>(null);
        }

        contact.FullName = request.FullName.Trim();
        contact.Relationship = request.Relationship.Trim();
        contact.PhoneNumber = request.PhoneNumber.Trim();
        contact.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        contact.AccessLevel = request.AccessLevel;
        contact.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult<ContactResponse?>(MapToResponse(contact));
    }

    public Task<ContactResponse?> UpdateAccessLevelAsync(Guid userId, Guid contactId, UpdateContactAccessLevelRequest request)
    {
        var contact = FindOwned(userId, contactId);
        if (contact == null)
        {
            return Task.FromResult<ContactResponse?>(null);
        }

        contact.AccessLevel = request.AccessLevel;
        contact.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult<ContactResponse?>(MapToResponse(contact));
    }

    public Task<bool> DeleteAsync(Guid userId, Guid contactId)
    {
        var contact = FindOwned(userId, contactId);
        return Task.FromResult(contact != null && _contacts.TryRemove(contactId, out _));
    }

    public Task DeleteAllByUserAsync(Guid userId)
    {
        foreach (var id in _contacts.Values.Where(c => c.UserId == userId).Select(c => c.Id).ToList())
        {
            _contacts.TryRemove(id, out _);
        }

        return Task.CompletedTask;
    }

    private TrustContact? FindOwned(Guid userId, Guid contactId)
    {
        return _contacts.TryGetValue(contactId, out var contact) && contact.UserId == userId ? contact : null;
    }

    private static ContactResponse MapToResponse(TrustContact c) => new()
    {
        Id = c.Id,
        UserId = c.UserId,
        FullName = c.FullName,
        Relationship = c.Relationship,
        PhoneNumber = c.PhoneNumber,
        Email = c.Email,
        AccessLevel = c.AccessLevel.ToString(),
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}

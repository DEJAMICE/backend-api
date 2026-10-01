using SafeSignal.Api.Domain.Enums;

namespace SafeSignal.Api.Domain.Entities;

/// <summary>
/// Entidad del Dominio: Contacto de confianza de un usuario (red de apoyo).
/// </summary>
public class TrustContact
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public AccessLevel AccessLevel { get; set; } = AccessLevel.Secondary;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

namespace SafeSignal.Api.DTOs.Contacts;

/// <summary>
/// DTO de Respuesta: Contacto de confianza.
/// </summary>
public class ContactResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string AccessLevel { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

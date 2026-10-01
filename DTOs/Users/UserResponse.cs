namespace SafeSignal.Api.DTOs.Users;

/// <summary>
/// DTO de Respuesta: Datos públicos del perfil de usuario (nunca incluye la contraseña).
/// </summary>
public class UserResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string ProfileType { get; set; } = string.Empty;
    public string SubscriptionPlan { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

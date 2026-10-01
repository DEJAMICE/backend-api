using SafeSignal.Api.Domain.Enums;

namespace SafeSignal.Api.Domain.Entities;

/// <summary>
/// Entidad del Dominio: Usuario registrado en SafeSignal.
/// </summary>
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserProfile ProfileType { get; set; } = UserProfile.Standard;
    public SubscriptionPlan SubscriptionPlan { get; set; } = SubscriptionPlan.Free;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

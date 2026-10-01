using SafeSignal.Api.Domain.Enums;

namespace SafeSignal.Api.DTOs.Users;

/// <summary>
/// DTO de Solicitud: Cambio del plan de suscripción (Free / Premium).
/// </summary>
public class UpdateSubscriptionRequest
{
    public SubscriptionPlan Plan { get; set; } = SubscriptionPlan.Free;
}

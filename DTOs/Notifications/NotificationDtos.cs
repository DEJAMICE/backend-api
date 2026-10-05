namespace SafeSignal.Api.DTOs.Notifications;

/// <summary>
/// DTO de respuesta para una notificación de usuario.
/// </summary>
public class NotificationResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "Sistema";
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ActionUrl { get; set; }
}

/// <summary>
/// DTO de solicitud para crear una notificación.
/// </summary>
public class CreateNotificationRequest
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "Sistema";
    public string? ActionUrl { get; set; }
}

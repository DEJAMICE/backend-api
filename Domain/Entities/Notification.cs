namespace SafeSignal.Api.Domain.Entities;

/// <summary>
/// Entidad del Dominio: Notificación del sistema o de seguridad dirigida a un usuario.
/// </summary>
public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "Sistema"; // "Alerta", "Ruta", "Dispositivo", "Reporte", "Sistema"
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ActionUrl { get; set; }
}

using System.Collections.Concurrent;
using SafeSignal.Api.Domain.Entities;
using SafeSignal.Api.DTOs.Notifications;

namespace SafeSignal.Api.Services;

/// <summary>
/// Implementación del servicio de notificaciones con almacenamiento seguro en memoria.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly ConcurrentDictionary<Guid, Notification> _notifications = new();

    public NotificationService()
    {
        // Notificaciones iniciales semilla para el usuario demo
        var seeds = new[]
        {
            new Notification
            {
                Id = Guid.NewGuid(),
                UserId = UserService.DemoUserId,
                Title = "Dispositivo IoT Vinculado",
                Message = "El Botón de Pánico Físico SN-001 se ha sincronizado correctamente vía Bluetooth.",
                Type = "Dispositivo",
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddMinutes(-25),
                ActionUrl = "/app/dispositivos"
            },
            new Notification
            {
                Id = Guid.NewGuid(),
                UserId = UserService.DemoUserId,
                Title = "Alerta Comunitaria en tu zona",
                Message = "Se ha reportado un incidente de arrebato cerca a tu ruta habitual en Av. Salaverry.",
                Type = "Alerta",
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                ActionUrl = "/app/reportes"
            },
            new Notification
            {
                Id = Guid.NewGuid(),
                UserId = UserService.DemoUserId,
                Title = "Bienvenido a SafeSignal",
                Message = "Tu red de confianza está lista para protegerte. Agrega hasta 5 contactos prioritarios.",
                Type = "Sistema",
                IsRead = true,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                ActionUrl = "/app/contactos"
            }
        };

        foreach (var n in seeds)
        {
            _notifications.TryAdd(n.Id, n);
        }
    }

    public Task<IEnumerable<NotificationResponse>> GetByUserIdAsync(Guid userId)
    {
        // Si el usuario no tiene notificaciones aún, sembramos una de bienvenida
        var userNotifs = _notifications.Values
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToList();

        if (userNotifs.Count == 0)
        {
            var welcome = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = "Bienvenido a SafeSignal",
                Message = "Tu cuenta está activa. Configura tus contactos de confianza y rutas seguras.",
                Type = "Sistema",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                ActionUrl = "/app/contactos"
            };
            _notifications.TryAdd(welcome.Id, welcome);
            userNotifs.Add(welcome);
        }

        var responses = userNotifs.Select(MapToResponse);
        return Task.FromResult<IEnumerable<NotificationResponse>>(responses);
    }

    public Task<int> GetUnreadCountAsync(Guid userId)
    {
        var count = _notifications.Values.Count(n => n.UserId == userId && !n.IsRead);
        return Task.FromResult(count);
    }

    public Task<bool> MarkAsReadAsync(Guid id, Guid userId)
    {
        if (_notifications.TryGetValue(id, out var notif) && notif.UserId == userId)
        {
            notif.IsRead = true;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<int> MarkAllAsReadAsync(Guid userId)
    {
        int count = 0;
        foreach (var notif in _notifications.Values.Where(n => n.UserId == userId && !n.IsRead))
        {
            notif.IsRead = true;
            count++;
        }
        return Task.FromResult(count);
    }

    public Task<NotificationResponse> CreateAsync(Guid userId, CreateNotificationRequest request)
    {
        var notif = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title.Trim(),
            Message = request.Message.Trim(),
            Type = string.IsNullOrWhiteSpace(request.Type) ? "Sistema" : request.Type.Trim(),
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            ActionUrl = request.ActionUrl
        };

        _notifications.TryAdd(notif.Id, notif);
        return Task.FromResult(MapToResponse(notif));
    }

    public Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        if (_notifications.TryGetValue(id, out var notif) && notif.UserId == userId)
        {
            return Task.FromResult(_notifications.TryRemove(id, out _));
        }
        return Task.FromResult(false);
    }

    private static NotificationResponse MapToResponse(Notification n) => new()
    {
        Id = n.Id,
        UserId = n.UserId,
        Title = n.Title,
        Message = n.Message,
        Type = n.Type,
        IsRead = n.IsRead,
        CreatedAt = n.CreatedAt,
        ActionUrl = n.ActionUrl
    };
}

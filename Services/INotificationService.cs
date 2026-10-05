using SafeSignal.Api.DTOs.Notifications;

namespace SafeSignal.Api.Services;

public interface INotificationService
{
    Task<IEnumerable<NotificationResponse>> GetByUserIdAsync(Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task<bool> MarkAsReadAsync(Guid id, Guid userId);
    Task<int> MarkAllAsReadAsync(Guid userId);
    Task<NotificationResponse> CreateAsync(Guid userId, CreateNotificationRequest request);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}

using Domain.Enums;

namespace Application.Common.Interfaces.Notifications
{
    public interface IInAppNotificationService
    {
        Task SendToUserAsync(Guid recipientId, string title, string message, NotificationType type, Guid? targetId = null, string? targetType = null);
        Task BroadcastAsync(IEnumerable<Guid> recipientIds, string title, string message, NotificationType type, Guid? targetId = null, string? targetType = null);
        Task MarkAsReadAsync(Guid notificationId);
    }
}
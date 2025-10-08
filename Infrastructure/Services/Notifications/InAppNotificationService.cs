using Application.Common.Interfaces.Notifications;
using Application.Common.Interfaces.Realtime;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Notifications
{
    public class InAppNotificationService : IInAppNotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IRealtimeNotifier _realtimeNotifier;
        private readonly ILogger<InAppNotificationService> _logger;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InAppNotificationService(INotificationRepository notificationRepository, IRealtimeNotifier realtimeNotifier, ILogger<InAppNotificationService> logger, IAuditLogRepository auditLogRepository, IUnitOfWork unitOfWork)
        {
            _notificationRepository = notificationRepository;
            _realtimeNotifier = realtimeNotifier;
            _logger = logger;
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task SendToUserAsync(Guid recipientId, string title, string message, NotificationType type, Guid? targetId = null, string? targetType = null)
        {
            var notification = new Notification(recipientId, title, message, type, targetId, targetType);

            await _notificationRepository.AddAsync(notification);

            var audit = new AuditLog(
                AuditActionType.Created,
                nameof(Notification),
                notification.Id,
                $"Notification sent to user {recipientId} | Title: {title}",
                userId: recipientId
            );

            await _auditLogRepository.AddAsync(audit);
            await _unitOfWork.SaveChangesAsync();

            await _realtimeNotifier.SendToUserAsync(recipientId, "ReceiveNotification", new
            {
                notification.Id,
                notification.Title,
                notification.Message,
                notification.Type,
                notification.CreatedAt
            });

            _logger.LogInformation("Notification saved and sent to user {UserId}", recipientId);
        }

        public async Task BroadcastAsync(IEnumerable<Guid> recipientIds, string title, string message,
            NotificationType type, Guid? targetId = null, string? targetType = null)
        {
            var notifications = recipientIds.Select(id => new Notification(id, title, message, type, targetId, targetType)).ToList();

            await _notificationRepository.AddAsync(notifications);

            var audit = new AuditLog(
                AuditActionType.Created,
                nameof(Notification),
                null,
                $"Broadcast notification sent to {recipientIds.Count()} users | Title: {title}"
            );

            await _auditLogRepository.AddAsync(audit);
            await _unitOfWork.SaveChangesAsync();

            await _realtimeNotifier.SendToUsersAsync(recipientIds, "ReceiveNotification", new
            {
                Title = title,
                Message = message,
                Type = type.ToString(),
                TargetId = targetId,
                TargetType = targetType
            });

            _logger.LogInformation("Broadcast notification sent to {Count} users", recipientIds.Count());
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _notificationRepository.GetAsync(notificationId);
            if (notification == null)
            {
                _logger.LogWarning("Notification {NotificationId} not found", notificationId);
                return;
            }

            notification.MarkAsRead();
            notification.MarkUpdated();

            var audit = new AuditLog(
                AuditActionType.Updated,
                nameof(Notification),
                notification.Id,
                "User marked notification as read",
                notification.RecipientId
            );

            await _auditLogRepository.AddAsync(audit);
            await _notificationRepository.UpdateAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Notification {NotificationId} marked as read", notificationId);
        }
    }
}
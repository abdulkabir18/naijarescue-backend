using Application.Common.Interfaces.Notifications;
using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IInAppNotificationService _inAppNotificationService;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IUserRepository userRepository, IEmailService emailService, IInAppNotificationService inAppNotificationService, IAuditLogRepository auditLogRepository, IUnitOfWork unitOfWork, ILogger<NotificationService> logger)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _inAppNotificationService = inAppNotificationService;
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task NotifyEmergencyContactsAsync(Guid userId, Incident incident)
        {
            var contactEmails = await _userRepository.GetEmergencyContactEmailsAsync(userId);
            if (!contactEmails.Any())
            {
                _logger.LogWarning("No emergency contacts found for user {UserId}", userId);
                return;
            }

            var subject = "🚨 Emergency Alert from NaijaRescue";
            var body = $@"
                <h3>Emergency Alert!</h3>
                <p><strong>Incident Type:</strong> {incident.Type}</p>
                <p><strong>Location:</strong> {incident.Location}</p>
                <p><strong>Time:</strong> {incident.CreatedAt}</p>
                <p>Please reach out or take necessary action immediately.</p>
            ";

            await _emailService.SendEmailAsync(contactEmails, subject, body);

            await _auditLogRepository.AddAsync(new AuditLog(
                AuditActionType.Created,
                nameof(Incident),
                incident.Id,
                $"Emergency contacts notified for incident {incident.Id}",
                userId
            ));

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Emergency contacts notified for incident {IncidentId}", incident.Id);
        }

        public async Task SendUserNotificationAsync(Guid userId, string title, string message)
        {
            await _inAppNotificationService.SendToUserAsync(
                userId,
                title,
                message,
                NotificationType.System
            );

            var audit = new AuditLog(
                AuditActionType.Created,
                nameof(Notification),
                null,
                $"User notification sent to {userId} | {title}",
                userId
            );
            await _auditLogRepository.AddAsync(audit);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task BroadcastAsync(IEnumerable<Guid> userIds, string title, string message)
        {
            await _inAppNotificationService.BroadcastAsync(
                userIds,
                title,
                message,
                NotificationType.System
            );

            var audit = new AuditLog(
                AuditActionType.Created,
                nameof(NotificationService),
                null,
                $"Broadcast notification sent to {userIds.Count()} users: {title}"

            );
            await _auditLogRepository.AddAsync(audit);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Broadcast notification sent | Title: {Title} | Recipients: {Count}", title, userIds.Count());
        }
    }
}

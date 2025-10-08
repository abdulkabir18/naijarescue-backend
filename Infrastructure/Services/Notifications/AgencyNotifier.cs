using Application.Common.Interfaces.Notifications;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Notifications
{
    public class AgencyNotifier : IAgencyNotifier
    {
        private readonly IInAppNotificationService _inAppNotificationService;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AgencyNotifier> _logger;

        public AgencyNotifier(
            IInAppNotificationService inAppNotificationService,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork,
            ILogger<AgencyNotifier> logger)
        {
            _inAppNotificationService = inAppNotificationService;
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


    }
}

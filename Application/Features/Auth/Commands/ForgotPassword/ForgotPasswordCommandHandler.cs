using Application.Common.Dtos;
using Application.Interfaces.CurrentUser;
using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IVerificationService _verificationService;
        private readonly IEmailService _emailService;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;

        public ForgotPasswordCommandHandler(IUserRepository userRepository, IVerificationService verificationService, IEmailService emailService, IAuditLogRepository auditLogRepository, IUnitOfWork unitOfWork, ILogger<ForgotPasswordCommandHandler> logger, ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _verificationService = verificationService;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _auditLogRepository = auditLogRepository;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetAsync(u => u.Email == new Email(request.Email));
                if (user == null)
                {
                    _logger.LogInformation("ForgotPassword requested for non-existent email: {Email}", request.Email);
                    return Result<bool>.Failure("Invalid email.");
                }
                else if (user.IsDeleted || !user.IsActive)
                {
                    _logger.LogInformation("ForgotPassword requested for inactive or deleted user: {Email}", request.Email);
                    return Result<bool>.Failure("User account is inactive or deleted.");
                }

                string verificationCode = await _verificationService.GenerateCodeAsync(user.Id, 15);

                string subject = "NaijaRescue Password Reset";
                string body = $@"
                <h3>NaijaRescue Password Reset</h3>
                <p>Hello {user.FullName ?? "User"},</p>
                <p>We received a request to reset your NaijaRescue account password. 
                Use the verification code below to continue:</p>

                <h2 style='color:#e63946; letter-spacing:2px;'>{verificationCode}</h2>

                <p>This code is valid for <b>15 minutes</b>. If you didn’t request this reset, 
                please ignore this email — your account is still secure.</p>

                <p>Stay safe,<br><b>The NaijaRescue Team 🚨</b></p>";

                await _emailService.SendEmailAsync(user.Email.Value, subject, body);

                await _auditLogRepository.AddAsync(new AuditLog(
                    AuditActionType.Security,
                    nameof(User),
                    user.Id,
                    "Password reset request initiated.",
                    user.Id,
                    _currentUserService.IpAddress,
                    _currentUserService.UserAgent
                ));
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Password reset code sent to email: {Email}", request.Email);
                return Result<bool>.Success(true, $"OTP to reset your password is been sent to this email {user.Email.Value}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling ForgotPasswordCommand for email {Email}", request.Email);
                return Result<bool>.Failure("An error occurred while processing your request.");
            }
        }
    }
}
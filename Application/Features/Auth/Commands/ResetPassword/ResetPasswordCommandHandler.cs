using Application.Common.Dtos;
using Application.Interfaces.CurrentUser;
using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Domain.Common.Security;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IVerificationService _verificationService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;
        private readonly IEmailService _emailService;

        public ResetPasswordCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService, IVerificationService verificationService, IUnitOfWork unitOfWork, ILogger<ResetPasswordCommandHandler> logger, IPasswordHasher passwordHasher, IAuditLogRepository auditLogRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _verificationService = verificationService;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _auditLogRepository = auditLogRepository;
            _emailService = emailService;
        }
        public async Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetAsync(u => u.Email == new Email(request.Email));
                if (user == null || !user.IsActive || user.IsDeleted)
                {
                    _logger.LogInformation("ResetPassword requested for non-existent email: {Email}", request.Email);
                    return Result<bool>.Failure("Invalid or inactive user.");
                }

                if(request.Code.Length != 6 || !int.TryParse(request.Code, out int r))
                {
                    _logger.LogInformation("ResetPasswordCommand code is not valid");
                    return Result<bool>.Failure("Invalid code");
                }

                bool isValidCode = await _verificationService.ValidateCodeAsync(user.Id, request.Code);
                if (!isValidCode)
                {
                    _logger.LogWarning("Invalid or expired password reset code for user {UserId}", user.Id);
                    return Result<bool>.Failure("Invalid or expired reset code.");
                }

                if (request.NewPassword != request.ConfirmPassword)
                    return Result<bool>.Failure("Password did not match");

                user.ChangePassword(request.NewPassword, _passwordHasher);

                await _userRepository.UpdateAsync(user);

                var audit = new AuditLog(
                    AuditActionType.Security,
                    nameof(User),
                    user.Id,
                    "Password successfully reset.",
                    user.Id,
                    _currentUserService.IpAddress,
                    _currentUserService.UserAgent
                );
                await _auditLogRepository.AddAsync(audit);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                string subject = "✅ Password Reset Successful";
                string body = $@"
                    <h3>Password Reset Confirmation</h3>
                    <p>Hello {user.FullName ?? "User"},</p>
                    <p>Your password has been successfully reset. 
                    If you did not initiate this action, please contact support immediately.</p>
                    <p>Stay safe,<br><b>The NaijaRescue Team 🚨</b></p>";

                await _emailService.SendEmailAsync(user.Email.Value, subject, body);

                _logger.LogInformation("Password reset successful for user {Email}", request.Email);

                return Result<bool>.Success(true, "Password has been reset successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ResetPasswordCommand for email {Email}", request.Email);
                return Result<bool>.Failure("An error occurred while resetting the password.");
            }
        }
    }
}

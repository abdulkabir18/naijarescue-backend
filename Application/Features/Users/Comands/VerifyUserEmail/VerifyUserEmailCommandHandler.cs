using Application.Common.Dtos;
using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using MediatR;

namespace Application.Features.Users.Comands.VerifyUserEmail
{
    public class VerifyUserEmailCommandHandler : IRequestHandler<VerifyUserEmailCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IVerificationService _verificationService;
        private readonly IUnitOfWork _unitOfWork;

        public VerifyUserEmailCommandHandler(IUserRepository userRepository, IVerificationService verificationService, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _verificationService = verificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(VerifyUserEmailCommand request, CancellationToken cancellationToken)
        {
            if(request.Model == null || request == null)
                return Result<bool>.Failure("Invalid request payload.");
            var user = await _userRepository.GetUserByEmailAsync(request.Model.Email);
            if (user == null)
                return Result<bool>.Failure("User not found.");
            var isCodeValid = await _verificationService.ValidateCodeAsync(user.Id, request.Model.Code);
            if (!isCodeValid)
                return Result<bool>.Failure("Invalid or expired verification code.");

            user.VerifyEmail();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true, "User email verified successfully.");
        }
    }
}

using MediatR;
using Security.Application.Abstractions.Persistence;
using Security.Application.Abstractions.Security;
using Security.Application.Abstractions.Time;
using Security.Application.Abstractions.UnitOfWork;
using Security.Application.Auth.Dtos;
using Security.Application.Common.Errors;
using Security.Application.Common.Results;
using Security.Domain.Auditing;
using Security.Domain.Users;

namespace Security.Application.Auth.Register;

public sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IAuditLogRepository auditLogRepository,
    IPasswordHasher passwordHasher,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    public async Task<Result<RegisterResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();

        var alreadyExists = await userRepository.ExistsByNormalizedEmailAsync(normalizedEmail, cancellationToken);
        if (alreadyExists)
        {
            return Result<RegisterResponse>.Failure(AuthErrors.UserAlreadyExists);
        }

        var utcNow = dateTimeProvider.UtcNow;

        var user = new User(
            Guid.NewGuid(),
            request.Email.Trim(),
            normalizedEmail,
            passwordHasher.Hash(request.Password),
            utcNow);

        await userRepository.AddAsync(user, cancellationToken);

        var auditLog = new AuditLog(
            Guid.NewGuid(),
            user.Id,
            AuditActionType.UserRegistered,
            "system",
            "application",
            Guid.NewGuid().ToString("N"),
            """{"event":"user_registered"}""",
            utcNow);

        await auditLogRepository.AddAsync(auditLog, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new RegisterResponse(
            new UserDto(
                user.Id,
                user.Email,
                user.EmailVerified,
                user.IsActive));

        return Result<RegisterResponse>.Success(response);
    }
}
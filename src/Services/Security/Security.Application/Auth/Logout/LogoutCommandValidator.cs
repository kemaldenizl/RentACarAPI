using FluentValidation;

namespace Security.Application.Auth.Logout;

public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.SessionId).NotEmpty();

        RuleFor(x => x.AccessTokenJti)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.AccessTokenExpiresAtUtc)
            .NotEmpty();

        RuleFor(x => x.DeviceName)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.IpAddress)
            .NotEmpty()
            .MaximumLength(128);
    }
}
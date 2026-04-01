using FluentValidation;

namespace Security.Application.Sessions.RevokeSession;

public sealed class RevokeSessionCommandValidator : AbstractValidator<RevokeSessionCommand>
{
    public RevokeSessionCommandValidator()
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
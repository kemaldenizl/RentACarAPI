using FluentValidation;

namespace Security.Application.Auth.Logout;

public sealed class LogoutAllCommandValidator : AbstractValidator<LogoutAllCommand>
{
    public LogoutAllCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.AccessTokenJti)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.AccessTokenExpiresAtUtc)
            .NotEmpty();
    }
}
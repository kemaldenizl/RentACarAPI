using FluentValidation;

namespace Security.Application.Auth.Refresh;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .MaximumLength(2048);

        RuleFor(x => x.DeviceName)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.IpAddress)
            .NotEmpty()
            .MaximumLength(128);
    }
}
using FluentValidation;

namespace Security.Application.Auth.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(320)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.DeviceName)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.IpAddress)
            .NotEmpty()
            .MaximumLength(128);
    }
}
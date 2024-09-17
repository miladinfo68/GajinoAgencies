using FluentValidation;
using FluentValidation.Results;
using GajinoAgencies.Settings;

namespace GajinoAgencies.Dtos;

public record LoginRequestDto(string Username, string Password);
public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(u => u.Username)
            .NotNull().NotEmpty()
            .WithMessage("Username is required and can't be empty.")
            .Matches(AppConstants.MobileRegex).WithMessage("Mobile number must start with 0098, +98, or 0, followed by a 9 and 9 digits");

        RuleFor(u => u.Password)
            .NotNull().NotEmpty()
            .WithMessage("Password is required and can't be empty")
            .Length(6, 100).WithMessage("Password must be between 6 and 100 characters")
            .Matches(AppConstants.PasswordRegex).WithMessage("Password must contain at least one letter, one digit, and one special character");
    }

    public override ValidationResult Validate(ValidationContext<LoginRequestDto> context)
    {
        var result = base.Validate(context);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
        return result;
    }
}
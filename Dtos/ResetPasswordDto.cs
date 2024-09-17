using FluentValidation;
using FluentValidation.Results;
using GajinoAgencies.Settings;

namespace GajinoAgencies.Dtos;

public record ResetPasswordDto(string OtpCode, string Mobile, string Password, string ConfirmPassword);
public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidator()
    {
        RuleFor(u => u.OtpCode)
            .NotNull().NotEmpty()
            .WithMessage("OtpCode is required and can't be empty.")
            .Matches(AppConstants.OtpCodeRegex)
            .WithMessage("OtpCode must be exactly 4 digits.");

        RuleFor(u => u.Mobile)
            .NotNull().NotEmpty()
            .WithMessage("Mobile is required and can't be empty.")
            .Matches(AppConstants.MobileRegex).WithMessage("Mobile number must start with 0098, +98, or 0, followed by a 9 and 9 digits");

        RuleFor(u => u.Password)
            .NotNull().NotEmpty()
            .WithMessage("New Password is required and can't be empty")
            .Length(6, 100).WithMessage("NewPassword must be between 6 and 100 characters")
            .Matches(AppConstants.PasswordRegex).WithMessage("NewPassword must contain at least one letter, one digit, and one special character");

        RuleFor(u => u.ConfirmPassword)
           .NotNull().NotEmpty()
           .WithMessage("ConfirmPassword is required and can't be empty.")
           .Equal(u => u.Password)
           .WithMessage("ConfirmPassword must match Password.");
    }

    public override ValidationResult Validate(ValidationContext<ResetPasswordDto> context)
    {
        var result = base.Validate(context);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
        return result;
    }
}







using FluentValidation;
using FluentValidation.Results;
using GajinoAgencies.Settings;
using System.Text.Json.Serialization;

namespace GajinoAgencies.Dtos;

public record ChangePasswordDto(string OldPassword ,string NewPassword ,string ConfirmPassword)
{
    [JsonIgnore] public int? UserId { get; set; } = null;
}

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(u => u.OldPassword)
            .NotNull().NotEmpty()
            .WithMessage("OldPassword is required and can't be empty.");
            //.Matches(AppConstants.MobileRegex).WithMessage("Mobile number must start with 0098, +98, or 0, followed by a 9 and 9 digits");

        RuleFor(u => u.NewPassword)
            .NotNull().NotEmpty()
            .WithMessage("NewPassword is required and can't be empty")
            .Length(6, 100).WithMessage("NewPassword must be between 6 and 100 characters")
            .Matches(AppConstants.PasswordRegex).WithMessage("NewPassword must contain at least one letter, one digit, and one special character");

        RuleFor(u => u.ConfirmPassword)
           .NotNull().NotEmpty()
           .WithMessage("ConfirmPassword is required and can't be empty.")
           .Equal(u => u.NewPassword)
           .WithMessage("ConfirmPassword must match NewPassword.");
    }

    public override ValidationResult Validate(ValidationContext<ChangePasswordDto> context)
    {
        var result = base.Validate(context);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
        return result;
    }
}







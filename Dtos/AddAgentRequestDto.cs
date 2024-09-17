using FluentValidation;
using FluentValidation.Results;
using GajinoAgencies.Data;
using GajinoAgencies.Settings;
using System.Net.Mail;

namespace GajinoAgencies.Dtos;

public record AddAgentRequestDto(
     string Mobile,
     string Password,
     int LocationId,
     string? FirstName = default,
     string? LastName = default,
     string? Institute = default,
     string? Email = default);

public class AddAgentRequestDtoValidator : AbstractValidator<AddAgentRequestDto>
{
    private readonly AgencyDbContext _ctx;
    public AddAgentRequestDtoValidator(AgencyDbContext ctx)
    {
        _ctx = ctx;

        RuleFor(u => u.Mobile)
            .NotNull().NotEmpty()
            .WithMessage("Mobile is required and can't be empty.")
            .Matches(AppConstants.MobileRegex).WithMessage("Mobile number must start with 0098, +98, or 0, followed by a 9 and 9 digits");

        RuleFor(u => u.Email)
            .Must(email => string.IsNullOrEmpty(email?.Trim()) || IsValidEmail(email))
            .WithMessage("Email must be in a valid format if provided.");

        RuleFor(u => u.Password)
            .NotNull().NotEmpty()
            .WithMessage("Password is required and can't be empty")
            .Length(6, 100).WithMessage("Password must be between 6 and 100 characters")
            .Matches(AppConstants.PasswordRegex).WithMessage("Password must contain at least one letter, one digit, and one special character");

        RuleFor(u => u.LocationId)
            .NotNull().NotEmpty()
            .WithMessage("LocationId is required and can't be empty")
            .Must((dto, stopToken) =>
            {
                return dto.LocationId > 0 && _ctx.Locations.Any(a => a.Id == dto.LocationId);
            }).WithMessage("Invalid LocationId,Location not found");
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var m = new MailAddress(email);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
        //return Regex.IsMatch(email, AppConstants.EmailRegex);
    }

    public override ValidationResult Validate(ValidationContext<AddAgentRequestDto> context)
    {
        var result = base.Validate(context);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
        return result;
    }
}







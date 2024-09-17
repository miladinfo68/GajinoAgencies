using FluentValidation;
using FluentValidation.Results;
using GajinoAgencies.Settings;


namespace GajinoAgencies.Dtos;

public record SendOtpResponseDto(string Mobile, string OtpCode, DateTime SendDateTime, int MinuteExpirationTime);
public record SendOtpApiResponseDto(string Mobile, DateTime SendDateTime, int MinuteExpirationTime);

public record SendOtpRequestDto(string Mobile);
public class SendOtpRequestDtoValidator : AbstractValidator<SendOtpRequestDto>
{
    public SendOtpRequestDtoValidator()
    {
        RuleFor(u => u.Mobile)
            .NotNull().NotEmpty()
            .WithMessage("MobileNumber is required and can't be empty.")
            .Matches(AppConstants.MobileRegex).WithMessage("Mobile number must start with 0098, +98, or 0, followed by a 9 and 9 digits");
    }

    public override ValidationResult Validate(ValidationContext<SendOtpRequestDto> context)
    {
        var result = base.Validate(context);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
        return result;
    }
}
using FluentValidation;
using FluentValidation.Results;
using GajinoAgencies.Settings;

namespace GajinoAgencies.Dtos;


public record PaymentDocumentsResponseDto(
    string FirstName,
    string LastName,
    DateTime PaymentDate,
    decimal Deposit,
    string AccountNo,
    string TraceNo,
    int AgencyId);


public record AddPaymentDocumentDto(
    string FirstName,
    string LastName,
    DateTime PaymentDate,
    decimal Deposit,
    string AccountNo,
    string TraceNo,
    int AgencyId
    );

public class AddPaymentDocumentDtoValidator : AbstractValidator<AddPaymentDocumentDto>
{
    public AddPaymentDocumentDtoValidator()
    {
        RuleFor(v => v.FirstName)
            .NotNull().NotEmpty()
            .MinimumLength(3).WithMessage("FirstName is required and can't be empty");

        RuleFor(v => v.LastName)
            .NotNull().NotEmpty()
            .MinimumLength(3).WithMessage("LastName is required and can't be empty");

        RuleFor(v => v.PaymentDate)
            .NotNull().NotEmpty()
            .WithMessage("PaymentDate is required and can't be empty")
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage("PaymentDate must not be greater than the current date.");

        RuleFor(u => u.Deposit)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Deposit must be a non-negative value.")
            .PrecisionScale(20, 2, false)
            .WithMessage("Deposit must be a decimal value with a maximum of 18 digits and 2 decimal places.");

        RuleFor(u => u.AccountNo)
            .NotNull().NotEmpty()
            .WithMessage("AccountNo is required.")
            .Matches(AppConstants.AccountNoRegex)
            .WithMessage("AccountNo must be a maximum of 20 digits.");

        RuleFor(u => u.TraceNo)
            .NotNull().NotEmpty()
            .WithMessage("TraceNo is required.")
            .Matches(AppConstants.AccountNoRegex)
            .WithMessage("TraceNo must be a maximum of 20 digits.");

        RuleFor(v => v.AgencyId)
            .NotNull().NotEmpty()
            .GreaterThan(0).WithMessage("AgencyId is required and must be greater than 0");
    }

    public override ValidationResult Validate(ValidationContext<AddPaymentDocumentDto> context)
    {
        var result = base.Validate(context);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
        return result;
    }
}
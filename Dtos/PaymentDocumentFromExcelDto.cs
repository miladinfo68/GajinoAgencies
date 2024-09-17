using FluentValidation;
using GajinoAgencies.Settings;

namespace GajinoAgencies.Dtos;



public record ImportExcelResponseDto(List<PaymentDocumentFromExcelDto> inValidDocuments);

public record PaymentDocumentInDb(int AgencyId ,DateTime PaymentDate ,decimal Deposit ,string AccountNo ,string TraceNo);

public record PaymentDocumentFromExcelDto(
    string FirstName,
    string LastName,
    string PaymentDate,
    string Deposit,
    string AccountNo,
    string TraceNo,
    string AgencyId
);

public class PaymentDocumentFromExcelDtoValidator : AbstractValidator<PaymentDocumentFromExcelDto>
{
    public PaymentDocumentFromExcelDtoValidator()
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
            .Must(BeAValidDate)
            .WithMessage("PaymentDate must be a valid datetime, or can't be greater than the current date");

        RuleFor(u => u.Deposit)
            //.GreaterThanOrEqualTo(0)
            .Must(BeAValidDecimal)
            .WithMessage("Deposit must be a valid number or can't be a negative number");
        //.PrecisionScale(20, 2, false)
        //.WithMessage("Deposit must be a decimal value with a maximum of 18 digits and 2 decimal places");

        RuleFor(u => u.AccountNo)
            .NotNull().NotEmpty()
            .WithMessage("AccountNo is required")
            .Matches(AppConstants.AccountNoRegex)
            .WithMessage("AccountNo must be a maximum of 20 digits");

        RuleFor(u => u.TraceNo)
            .NotNull().NotEmpty()
            .WithMessage("TraceNo is required")
            .Matches(AppConstants.AccountNoRegex)
            .WithMessage("TraceNo must be a maximum of 20 digits");

        RuleFor(v => v.AgencyId)
            .NotNull().NotEmpty()
            //.GreaterThan(0)
            .Must(BeAValidInt)
            .WithMessage("AgencyId is required and must be greater than 0");
    }

  

    //public override ValidationResult Validate(ValidationContext<PaymentDocumentFromExcelDto> context)
    //{
    //    var result = base.Validate(context);
    //    if (!result.IsValid)
    //    {
    //        throw new ValidationException(result.Errors);
    //    }
    //    return result;
    //}

    
    private static bool BeAValidDate(string paymentDate)
    {
        var success = DateTime.TryParse(paymentDate, out var parsedDate);
        var res= success && parsedDate <= DateTime.Now;
        return res;
    }

    private static bool BeAValidDecimal(string decimalString)
    {
        var success = decimal.TryParse(decimalString, out var paredDecimal);
        var res= success && paredDecimal >= 0; 
        return res;
    }

    private static bool BeAValidInt(string intString)
    {
        var success = int.TryParse(intString, out var paredInt);
        var res= success && paredInt > 0;
        return res;
    }


}
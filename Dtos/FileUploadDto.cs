using FluentValidation;
using FluentValidation.Results;
using GajinoAgencies.Settings;

namespace GajinoAgencies.Dtos;

public record FileUploadDto(IFormFile File);

public class FileUploadDtoValidator : AbstractValidator<FileUploadDto>
{
    public FileUploadDtoValidator()
    {
        RuleFor(x => x.File)
            .NotNull().NotEmpty()
            .WithMessage("The file ir required and  cannot be empty.")
            .Must(BeAValidExcelFile).WithMessage("The file must be an Excel file (xlsx or xls).")
            .Must(HaveNonZeroFileSize).WithMessage("The file size must be greater than zero.");
    }


    public override ValidationResult Validate(ValidationContext<FileUploadDto> context)
    {
        var result = base.Validate(context);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
        return result;
    }

    private static bool BeAValidExcelFile(IFormFile file)
    {
        var extension = Path.GetExtension(file?.FileName)?.ToLowerInvariant();
        return AppConstants.ExcelExtensionsFormat.Split("|").Contains(extension);
    }

    private static bool HaveNonZeroFileSize(IFormFile file)
    {
        return file?.Length > 0;
    }
}
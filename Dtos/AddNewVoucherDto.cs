using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.Results;
using GajinoAgencies.Services;

namespace GajinoAgencies.Dtos;

public record AddNewVoucherDto(string Title, int Count, byte Discount, string Expiration, List<string> PackageDetailIds)
{
    [JsonIgnore] public int? AgencyId { get; set; } = null;
    [JsonIgnore] public string? CityCode { get; set; } = null;
    [JsonIgnore] public string? Code { get; set; } = null;
    [JsonIgnore] public string? Mobile { get; set; } = null;


    public static implicit operator AddVoucherByAgencyDto(AddNewVoucherDto dto)
    {
        return new AddVoucherByAgencyDto(
            dto.Title,
            dto.Count,
            dto.Discount,
            dto.Code!,
            DateTime.Parse(dto.Expiration),
           string.Join(",", dto.PackageDetailIds),
            dto.Mobile!,
            dto.AgencyId!.Value,
            dto.CityCode!
            );
    }
}

public class AddNewVoucherDtoValidator : AbstractValidator<AddNewVoucherDto>
{
    private readonly IGaginoService _gagino;

    public AddNewVoucherDtoValidator(IGaginoService gagino)
    {
        _gagino = gagino;
        RuleFor(v => v.Title)
            .NotNull().NotEmpty()
            .MinimumLength(3).WithMessage("Voucher Title is required and can't be empty");

        RuleFor(v => v.Count)
            .NotNull().NotEmpty()
            .Must(x => x is >= 1 and <= 999)
            .WithMessage("Voucher Count can't be empty and must be between 1 to 999");

        RuleFor(v => v.Discount)
            //.NotNull().NotEmpty()
            .Must(x => x is >= 0 and <= 50)
            .WithMessage("Maximum Voucher Discount is 50%");

        RuleFor(v => v.Expiration)
            .NotNull().NotEmpty()
            .WithMessage("Voucher Expiration is required and can't be empty")
            .Must(BeAValidDate)
            .WithMessage("Voucher Expiration must be a valid datetime, or can't be less than the current date");


        RuleFor(voucher => voucher.PackageDetailIds)
            .NotNull()
            .WithMessage("PackageDetailIds cannot be null.")
            .NotEmpty()
            .WithMessage("At least one PackageDetailIds must be provided.")
            .Must(BeValidPackageDetailIds)
            .WithMessage("All PackageDetailIds must be valid integers.")
            .Must(BeExistingPackageDetailIds)
            .WithMessage(voucher =>
            {
                var invalidPackageDetailIds = GetInvalidPackageDetailIds(voucher.PackageDetailIds);
                return $"The following PackageDetailIds do not exist: {invalidPackageDetailIds}";
            });
    }

    public override ValidationResult Validate(ValidationContext<AddNewVoucherDto> context)
    {
        var result = base.Validate(context);

        
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        return result;
    }

    private static bool BeAValidDate(string expirationDate)
    {
        var success = DateTime.TryParse(expirationDate, out var parsedDate);
        var res = success && parsedDate >= DateTime.Now;
        return res;
    }

    private static bool BeValidPackageDetailIds(IEnumerable<string> packageDetailIds)
    {
        var res = packageDetailIds.All(pId => int.TryParse(pId, out var res) && res > 0);
        return res;
    }

   

    private bool BeExistingPackageDetailIds(IEnumerable<string> packageDetailIds)
    {
        // Safely parse packageDetailIds to integers
        var inputPackageDetailIds = packageDetailIds
            .Select(pId => int.TryParse(pId, out var id) ? (int?)id : null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToList();

        var existPackageDetailIds = _gagino.GetAllActivePackages()
            .Select(s => s.PackageDetailId).Distinct().ToList();

        return !inputPackageDetailIds.Except(existPackageDetailIds).Any();
    }

    private string GetInvalidPackageDetailIds(IEnumerable<string> packageDetailIds)
    {
        // Safely parse packageDetailIds to integers
        var inputPackageDetailIds = packageDetailIds
            .Select(pId => int.TryParse(pId, out var id) ? (int?)id : null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToList();

        var existPackageDetailIds = _gagino.GetAllActivePackages()
            .Select(s => s.PackageDetailId).Distinct().ToList();

        var invalidPackageDetailIds = inputPackageDetailIds.Except(existPackageDetailIds).ToList();
        return string.Join(", ", invalidPackageDetailIds);
    }


}
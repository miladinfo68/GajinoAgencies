using FluentValidation;
using FluentValidation.Results;
using GajinoAgencies.Data;

using Microsoft.EntityFrameworkCore;

namespace GajinoAgencies.Dtos;

public record AddLocationRequestDto(string CityCode, string? City = null, string? Province = null);

public class AddLocationRequestDtoValidator : AbstractValidator<AddLocationRequestDto>
{
    private readonly AgencyDbContext _ctx;
    public AddLocationRequestDtoValidator(AgencyDbContext ctx)
    {
        _ctx = ctx;

        RuleFor(u => u.CityCode)
            .NotNull().NotEmpty()
            .WithMessage("CityCode is required and can't be empty.")
            .Must((dto, stopToken) =>
            {
                var location=  _ctx.Locations.FirstOrDefault(a => a.CityCode == dto.CityCode);
                return dto.CityCode.Trim().Length == 3 && location == null;
            }).WithMessage("Invalid CityCode,CityCode must be exactly 3 alphabetical characters such as A,B,C,... and already not to be exist");
    }

    public override ValidationResult Validate(ValidationContext<AddLocationRequestDto> context)
    {
        var result = base.Validate(context);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
        return result;
    }
}




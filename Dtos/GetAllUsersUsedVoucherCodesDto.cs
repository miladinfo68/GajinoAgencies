using System.ComponentModel;
using System.Text.Json.Serialization;

namespace GajinoAgencies.Dtos;

public record GetAllUsersUsedVoucherCodesDto
{
    [DefaultValue(1)] public int PageNumber { get; set; }
    [DefaultValue(50)] public int PageSize { get; set; }
    [DefaultValue(null)] public string? VoucherCode { get; set; }
    [DefaultValue(null)] public string? StartDate { get; set; }
    [JsonIgnore] public string? CityCode { get; set; }
    [JsonIgnore] public int? AgencyId { get; set; }
}

public record GetAllUsersUsedVoucherCodeResponseDto(
    string FirstName,
    string LastName,
    string Sex,
    string MobileNumber,
    string GradeField,
    string AcademicYear,
    string VoucherCode,
    string PackageTitle,
    int PaidAmount,
    DateTime PackageStartDate,
    DateTime PackageEndDate,
    int CentralOfficeShare,
    int AgencyShare,
    DateTime ExpireDate);
//{
//    public DateTime? ExpireDate { get; set; } = null;
//}

public record GetAllUsersUsedVoucherCodeResponseWithPaginationDto(
    int TotalCount,
    IEnumerable<GetAllUsersUsedVoucherCodeResponseDto> UsedVouchers);

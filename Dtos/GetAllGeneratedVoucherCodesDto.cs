using System.ComponentModel;
using System.Text.Json.Serialization;

namespace GajinoAgencies.Dtos;

public record GetAllGeneratedVoucherCodesDto
{
    [DefaultValue(1)] public int PageNumber { get; set; }
    [DefaultValue(50)] public int PageSize { get; set; }
    [DefaultValue(null)] public string? StartDate { get; set; }
    [JsonIgnore] public string? CityCode { get; set; }
    [JsonIgnore] public int? AgencyId { get; set; }
}

public record GetAllGeneratedVoucherCodesResponseDto(
    string VoucherTitle,
    string VoucherCode,
    int VoucherDiscount,
    int VoucherCount,
    int UsedCount,
    int UnUsedCount,
    DateTime? StartDateTime,
    DateTime? PackageEndDate,
    DateTime VoucherGeneratedCodeDate,
    string Status,
    DateTime ExpireDate);
//{
//    public DateTime? ExpireDate { get; set; } = null;
//}


public record GetAllGeneratedVoucherCodesResponseWithPaginationDto(
    int TotalCount, IEnumerable<GetAllGeneratedVoucherCodesResponseDto> vouchersList);
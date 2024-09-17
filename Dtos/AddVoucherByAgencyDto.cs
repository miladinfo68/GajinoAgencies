namespace GajinoAgencies.Dtos;

public record AddVoucherByAgencyDto(
    string VoucherTitle,
    int VoucherCount ,
    int VoucherDiscount ,
    string VoucherCode ,
    DateTime VoucherExpiration,
    string PackageDetailIds ,
    string UserMobile,
    int UserId ,
    string CityCode
);

public record RollbackAddVoucherByAgencyDto(int VoucherId, int VoucherGeneratedCodeId);
public record PackageDto(int PackageDetailId ,int PackageId, string Title, int AcademicYearId ,int GradeFieldId);
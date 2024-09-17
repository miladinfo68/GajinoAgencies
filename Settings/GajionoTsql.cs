namespace GajinoAgencies.Settings;

public static class GajionoTsql
{
    public const string SP_GetAllGeneratedVoucherCodes = "Usp_GetAllGeneratedVoucherCodes";
    public const string SP_GetAllUsersUsedVoucherCodes = "Usp_GetAllUsersUsedVoucherCodes";
    public const string SP_AddVoucherByAgency = "Usp_AddVoucherByAgency";
    public const string SP_RoolbackAddVoucherByAgency = "Usp_RoolbackAddVoucherByAgency";
    public const string SP_GetAllActivePackages = "Usp_GetAllActivePackagesForAgencies";
}

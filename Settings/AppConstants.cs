namespace GajinoAgencies.Settings;

public class AppConstants
{
    public const string OtpCodeRegex = @"^\d{4}$";
    public const string AccountNoRegex = @"^\d{1,20}$";
    public const string MobileRegex = @"^(0098|(\+98)|0)9\d{9}$";
    public const string PasswordRegex = @"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[\W_]).*$";
    public const string EmailRegex = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    public const string VoucherCodeRandomString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";

    public const string PersianDateRegex =
        @"^(?<year>\d{4})/(?<month>(0?[1-9]|1[0-2]))/(?<day>(0?[1-9]|[12][0-9]|30|31|29(?![0-9])))(\s(?<hour>(0?[0-9]|1[0-9]|2[0-3])):(?<minute>(0?[0-9]|[1-5][0-9])):(?<second>(0?[0-9]|[1-5][0-9]))?)?$";

    public const string PersianDateRegexTemplate =
        @"^(?<year>\d{4})(DELIMITER)(?<month>(0?[1-9]|1[0-2]))(DELIMITER)(?<day>(0?[1-9]|[12][0-9]|30|31|29(?![0-9])))(\s(?<hour>(0?[0-9]|1[0-9]|2[0-3])):(?<minute>(0?[0-9]|[1-5][0-9])):(?<second>(0?[0-9]|[1-5][0-9]))?)?$";

    public const string ExcelExtensionsFormat= ".xls|.xlsx";

}

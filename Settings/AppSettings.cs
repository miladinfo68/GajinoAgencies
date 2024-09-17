namespace GajinoAgencies.Settings;

public class AppSettings
{
    public JwtSettings? JwtSettings { get; set; }
    public CryptographySettings CryptographySettings { get; set; }
    public SmsProvider SmsProvider { get; set; }
    public OtpSettings OtpSettings { get; set; }
    public PackageCacheSettings PackageCacheSettings { get; set; }
    public AppConnectionStrings AppConnectionStrings { get; set; }
}

public class JwtSettings
{
    public string? FolderName { get; set; }
    public string? SecretKey { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public int MinuteExpirationTime { get; set; }
}


public class CryptographySettings
{
    public string Salt { get; set; }
    public int SaltSizeAsByte { get; set; }
}


public class SmsProvider
{
    public string Url { get; set; }
    public string BaseUrl { get; set; }
    public string Username{ get; set; }
    public string Password{ get; set; }
}

public class OtpSettings
{
    public int MinuteExpirationTime { get; set; }
    public string FolderName { get; set; }
}

public class PackageCacheSettings
{
    public string FolderName { get; set; }
    public int DayExpirationTime { get; set; }
}

public class AppConnectionStrings
{
    public string AgencyDb { get; set; }
    public string GajinoDb { get; set; }
    public string Redis { get; set; }
}




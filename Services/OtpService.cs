using GajinoAgencies.Dtos;
using GajinoAgencies.Settings;
using GajinoAgencies.Utilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Web;

namespace GajinoAgencies.Services;

public interface IOtpService
{
    ValueTask<string> Send(string mobile, CancellationToken stopToken = default);
    ValueTask<bool> IsValid(string mobile, string otpCode);

}

public class OtpService : IOtpService
{

    private readonly HttpClient _httpClient;
    private readonly ICacheManagerService _cacheManager;
    private readonly SmsProvider _smsProvider;
    private readonly OtpSettings _otpSettings;
    private Random _rnd;
    public OtpService(
        IHttpClientFactory clientFactory,
        ICacheManagerService cacheManager,
        IOptions<SmsProvider> smsProvider,
        IOptions<OtpSettings> otpSettings
         )
    {
        _cacheManager = cacheManager;
        _httpClient = clientFactory.CreateClient("OtpClient");
        _smsProvider = smsProvider.Value!;
        _otpSettings = otpSettings.Value;
    }
    public async ValueTask<string> Send(string mobile, CancellationToken stopToken = default)
    {

        var otpCode = GenerateOtp().ToString();
        var smsText = HttpUtility.UrlEncode(otpCode);

        var postData = new FormUrlEncodedContent(new[]
        {
                new KeyValuePair<string, string>("username", _smsProvider.Username),
                new KeyValuePair<string, string>("password", _smsProvider.Password),
                new KeyValuePair<string, string>("mobiles[0]", mobile),
                new KeyValuePair<string, string>("body", smsText)
            });

        var response = await _httpClient.PostAsync(_smsProvider.Url, postData, stopToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new SmsSendingException($"Failed to send SMS. Status Code: {response.StatusCode}", response.StatusCode);
        }
        var res = await response.Content.ReadAsStringAsync(stopToken);
        var jsonResponse = JObject.Parse(res);

        if (jsonResponse["ids"] != null && jsonResponse["ids"]!.HasValues)
        {
            return otpCode;
        }

        throw new SmsSendingException($"Failed to send SMS. Status Code: {response.StatusCode}, Response: {res}", response.StatusCode);
    }

    public async ValueTask<bool> IsValid(string mobile, string otpCode)
    {
        var otpObject = await _cacheManager.GetAsync<SendOtpResponseDto>($"{_otpSettings.FolderName}:{mobile}");
        if (otpObject == null) return false;
        return otpObject.OtpCode == otpCode
               && otpObject.SendDateTime.AddMinutes(otpObject.MinuteExpirationTime) >= DateTime.UtcNow;
    }

    private int GenerateOtp()
    {
        _rnd = new Random();
        return _rnd.Next(1000, 10000);
    }
}

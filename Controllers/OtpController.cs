using GajinoAgencies.Dtos;
using GajinoAgencies.Services;
using GajinoAgencies.Settings;
using GajinoAgencies.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GajinoAgencies.Controllers;


[Route("api/[controller]")]
[ApiController]

public class OtpController : ControllerBase
{
    private readonly IOtpService _otp;
    private readonly IUserAccountService _userAccount;
    private readonly ICacheManagerService _cacheManager;

    private readonly OtpSettings _otpSettings;

    public OtpController(
        IUserAccountService userAccount,
        IOtpService otp,
        ICacheManagerService cacheManager,
        IOptions<OtpSettings> otpSettings)
    {
        _otp = otp;
        _userAccount = userAccount;
        _cacheManager = cacheManager;
        _otpSettings = otpSettings.Value!;
    }



    [AllowAnonymous]
    [HttpPost("Send")]
    public async Task<ApiResponseDto<SendOtpApiResponseDto>> Send(SendOtpRequestDto dto)
    {
        //check if this user is a valid user or not
        var user = await _userAccount.GetUserByMobileNumber(dto.Mobile);

        var otpCode = await _otp.Send(dto.Mobile);

        var otpExpiration = TimeSpan.FromMinutes(_otpSettings.MinuteExpirationTime);

        var redisOtp = new SendOtpResponseDto(
            dto.Mobile,
            otpCode,
            DateTime.UtcNow,
            _otpSettings.MinuteExpirationTime);

        var optResponse =
            new SendOtpApiResponseDto(redisOtp.Mobile, redisOtp.SendDateTime, redisOtp.MinuteExpirationTime);

        await _cacheManager.SetAsync($"{_otpSettings.FolderName}:", dto.Mobile, redisOtp, otpExpiration);

        return ApiResponse.ResultMessage(optResponse);
    }
}

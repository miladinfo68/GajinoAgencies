using AutoMapper;
using GajinoAgencies.Dtos;
using GajinoAgencies.Models;
using GajinoAgencies.Services;
using GajinoAgencies.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GajinoAgencies.Controllers;



[Route("api/[controller]")]
[ApiController]

public class AccountsController : ControllerBase
{
    private readonly ILogger<AccountsController> _logger;
    private readonly ITokenManagerService _tokenManager;
    private readonly IUserAccountService _userAccount;
    private readonly IPasswordManagerService _passwordManager;
    private readonly IMapper _mapper;


    public AccountsController(
        ILogger<AccountsController> logger
        //,ITokenManagerService tokenManager,
        ,IUserAccountService accountService
        //,IPasswordManagerService passwordManager
        , IMapper mapper
        )
    {
        _logger = logger;
        //_tokenManager = tokenManager;
        _userAccount = accountService;
        _mapper = mapper;
        //_passwordManager = passwordManager;
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<ApiResponseDto<string>> Login(LoginRequestDto dto)
    {
        var token = await _userAccount.Login(dto);
        return ApiResponse.ResultMessage(token);
    }

    //[AllowAnonymous]
    //[HttpPost("Register")]
    //public async Task<ApiResponseDto<bool>> Register(AddAgentRequestDto dto)
    //{
    //    var res = await _userAccount.Register(dto);
    //    return ApiResponse.ResultMessage(res);
    //}

    [HttpPost("Logout")]
    public async Task<ApiResponseDto<bool>> Logout()
    {
        var (userId, cityCode, mobile) = _userAccount.GetUserInfoFromToken(User.Claims);
        if (userId <= 0) return ApiResponse.ResultMessage(false);
        var res = await _userAccount.Logout(userId.ToString());
        return ApiResponse.ResultMessage(res);
    }


    [HttpGet("User-Authority")]
    public async Task<ApiResponseDto<UserAuthorityResponseDto>> UserAuthority()
    {
        var (userId, cityCode, mobile) = _userAccount.GetUserInfoFromToken(User.Claims);
        var user=await _userAccount.GetUserByMobileNumber(mobile);
        var userResponse = _mapper.Map<UserAuthorityResponseDto>(user);
        return ApiResponse.ResultMessage(userResponse);
    }


    //[HttpPost("Change-Password")]
    //public async Task<ApiResponseDto<bool>> ChangePassword(ChangePasswordDto dto)
    //{
    //    dto.UserId = _userAccount.GetUserId(User.Claims);
    //    var res = await _userAccount.ChangePassword(dto);
    //    return ApiResponse.ResultMessage(res);
    //}


    [AllowAnonymous]
    [HttpPost("Reset-Password")]
    public async Task<ApiResponseDto<bool>> ResetPassword(ResetPasswordDto dto)
    {
        var res = await _userAccount.ResetPassword(dto);
        return ApiResponse.ResultMessage(res);
    }




    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    /*
  
    [AllowAnonymous]
    [HttpGet("Generate-Jwt-Token")]
    public ApiResponseDto<string> GenerateToken()
    {
        var token = _tokenManager.GenerateToken(new TokenRequestDto(1000, "09352453609", "LOR"));
        return ApiResponse.ResultMessage(token);
    }

    [AllowAnonymous]
    [HttpGet("Encrypt-Password")]
    public ApiResponseDto<string> EncryptPassword(string plainText)
    {
        var encryptedText = _passwordManager.Encrypt(plainText);
        return ApiResponse.ResultMessage(encryptedText);
    }

    [AllowAnonymous]
    [HttpGet("Decrypt-Password")]
    public ApiResponseDto<string> DecryptPassword(string encryptedText)
    {
        var plainText = _passwordManager.Decrypt(encryptedText);
        return ApiResponse.ResultMessage(plainText);
    }
 
    */
}

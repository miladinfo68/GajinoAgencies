using System.Collections;
using GajinoAgencies.Dtos;
using GajinoAgencies.Services;
using GajinoAgencies.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace GajinoAgencies.Controllers;


[Route("api/[controller]")]
[ApiController]

public class VouchersController : ControllerBase
{

    private readonly ILogger<AccountsController> _logger;
    private readonly IVoucherManagerService _voucherManager;
    private readonly IUserAccountService _userAccount;


    public VouchersController(ILogger<AccountsController> logger,
        IVoucherManagerService voucherManager,
        IUserAccountService userAccount)
    {
        _logger = logger;
        _voucherManager = voucherManager;
        _userAccount = userAccount;
    }
    

    [HttpPost("Get-All-Generated-VoucherCodes")]
    public async Task<ApiResponseDto<GetAllGeneratedVoucherCodesResponseWithPaginationDto>> GetAllGeneratedVoucherCodes(GetAllGeneratedVoucherCodesDto dto)
    {
        var (userId, cityCode, mobile) = _userAccount.GetUserInfoFromToken(User.Claims);
        dto.CityCode = cityCode;
        dto.AgencyId = userId;
        var vouchersList = await _voucherManager.GetAllGeneratedVoucherCodes(dto);
        var totalCount = vouchersList?.ToList()?.Count ?? 0;
        var res = new GetAllGeneratedVoucherCodesResponseWithPaginationDto(totalCount, vouchersList);
        return ApiResponse.ResultMessage(res);
    }

    [HttpPost("Get-All-Users-Used-VoucherCodes")]
    public async Task<ApiResponseDto<GetAllUsersUsedVoucherCodeResponseWithPaginationDto>> GetAllUsersUsedVoucherCodes(GetAllUsersUsedVoucherCodesDto dto)
    {
        var (userId, cityCode, mobile) = _userAccount.GetUserInfoFromToken(User.Claims);
        dto.CityCode = cityCode;
        dto.AgencyId = userId;
        var vouchersList = await _voucherManager.GetAllUsersUsedVoucherCodes(dto);
        var totalCount = vouchersList?.ToList()?.Count ?? 0;
        var res = new GetAllUsersUsedVoucherCodeResponseWithPaginationDto(totalCount, vouchersList);
        return ApiResponse.ResultMessage(res);
    }



    [HttpPost("Add")]
    public async Task<ApiResponseDto<bool>> Add(AddNewVoucherDto dto)
    {
        var (userId, cityCode, mobile) = _userAccount.GetUserInfoFromToken(User.Claims);

        dto.AgencyId = userId;
        dto.CityCode = cityCode;
        dto.Mobile = mobile;

        var res = await _voucherManager.AddNewVoucher(dto);

        return ApiResponse.ResultMessage(res);
    }




}

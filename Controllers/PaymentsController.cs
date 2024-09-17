 using GajinoAgencies.Dtos;
using GajinoAgencies.Services;
using GajinoAgencies.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GajinoAgencies.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : ControllerBase
{

    private readonly ILogger<PaymentsController> _logger;
    private readonly IPaymentDocumentService _paymentDocument;
    private readonly IUserAccountService _userAccount;

    public PaymentsController(
        ILogger<PaymentsController> logger,
        IPaymentDocumentService paymentDocument, 
        IUserAccountService userAccount)
    {
        _logger = logger;
        _paymentDocument=paymentDocument;
        _userAccount = userAccount;
    }

    //[AllowAnonymous]
        [Authorize(Policy = "AdminPolicy")]
    [HttpPost("Add")]

    public async Task<ApiResponseDto<bool>> Add(AddPaymentDocumentDto dto)
    {

        var res = await _paymentDocument.Add(dto);
        return ApiResponse.ResultMessage(res);
    }


    //[AllowAnonymous]
    [Authorize(Policy = "AdminPolicy")]
    [HttpPost("Import")]
    public async Task<ApiResponseDto<ImportExcelResponseDto>> ImportExcel([FromForm] FileUploadDto dto)
    {
        var res = await _paymentDocument.ImportExcel(dto.File);
        return ApiResponse.ResultMessage(res);
    }

    [HttpGet("List")]
    public async Task<ApiResponseDto<IEnumerable<PaymentDocumentsResponseDto>>> GetPayments()
    {
        var (userId, cityCode, mobile) = _userAccount.GetUserInfoFromToken(User.Claims);
        var res = await _paymentDocument.GetAgencyPaymentsById(userId);
        return ApiResponse.ResultMessage(res);
    }

}

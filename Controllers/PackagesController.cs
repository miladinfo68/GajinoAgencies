using GajinoAgencies.Dtos;
using GajinoAgencies.Services;
using GajinoAgencies.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace GajinoAgencies.Controllers;


[Route("api/[controller]")]
[ApiController]

public class PackagesController : ControllerBase
{

    private readonly ILogger<AccountsController> _logger;
    private readonly IGaginoService _agino;


    public PackagesController(ILogger<AccountsController> logger, IGaginoService agino)
    {
        _logger = logger;
        _agino = agino;
    }

    [HttpGet("Actives")]
    public async Task<ApiResponseDto<IEnumerable<PackageDto>>> GetAllActivePackages()
    {
        var res = await _agino.GetAllActivePackagesAsync();
        return ApiResponse.ResultMessage(res);
    }

    [HttpGet("Filter")]
    public async Task<ApiResponseDto<IEnumerable<PackageDto>>> FilterActivePackages(string? search = null)
    {
        var res = await _agino.FilterActivePackages(search);
        return ApiResponse.ResultMessage(res);
    }

}

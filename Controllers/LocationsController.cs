using GajinoAgencies.Dtos;
using GajinoAgencies.Services;
using GajinoAgencies.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace GajinoAgencies.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LocationsController : ControllerBase
{

    private readonly ILogger<AccountsController> _logger;
    private readonly ILocationService _locationService;

    public LocationsController(ILogger<AccountsController> logger, ILocationService locationService)
    {
        _logger = logger;
        _locationService = locationService;
    }

    [HttpPost("Add")]
    public async Task<ApiResponseDto<bool>> Add(AddLocationRequestDto dto)
    {

        var res = await _locationService.Add(dto);
        return ApiResponse.ResultMessage(res);
    }

    [HttpGet]
    public async Task<ApiResponseDto<IEnumerable<LocationResponseDto>>> GetAllLocation()
    {
        var res = await _locationService.GetAll();
        return ApiResponse.ResultMessage(res);
    }

}

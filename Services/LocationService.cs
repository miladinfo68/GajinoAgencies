using AutoMapper;
using GajinoAgencies.Data;
using GajinoAgencies.Dtos;
using GajinoAgencies.Models;
using Microsoft.EntityFrameworkCore;

namespace GajinoAgencies.Services;

public interface ILocationService
{
    ValueTask<bool> Add(AddLocationRequestDto dto, CancellationToken stopToken = default);
    ValueTask<IEnumerable<LocationResponseDto>> GetAll(CancellationToken stopToken = default);
}
public class LocationService : ILocationService
{
    private readonly AgencyDbContext _ctx;
    private readonly IMapper _mapper;

    public LocationService(AgencyDbContext ctx, IMapper mapper)
    {
        _ctx = ctx;
        _mapper = mapper;
    }
    public async ValueTask<bool> Add(AddLocationRequestDto dto, CancellationToken stopToken = default)
    {
        var newLocation = _mapper.Map<Location>(dto);
        _ctx.Locations.Add(newLocation);
        var res = await _ctx.SaveChangesAsync(stopToken) > 0;
        return res;
    }

    public async ValueTask<IEnumerable<LocationResponseDto>> GetAll(CancellationToken stopToken = default)
    {
        var locations = await _ctx.Locations.AsNoTracking().ToListAsync(stopToken);
        var result = _mapper.Map<IEnumerable<LocationResponseDto>>(locations);
        return result ?? Enumerable.Empty<LocationResponseDto>();
    }
}

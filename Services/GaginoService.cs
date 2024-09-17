using Dapper;
using GajinoAgencies.Dtos;
using GajinoAgencies.Settings;
using System.Data;
using Microsoft.Extensions.Options;
using System.Drawing.Printing;

namespace GajinoAgencies.Services;

public interface IGaginoService
{
    ValueTask<IEnumerable<GetAllUsersUsedVoucherCodeResponseDto>> GetAllUsersUsedVoucherCodes(GetAllUsersUsedVoucherCodesDto dto, CancellationToken stopToken = default);

    ValueTask<IEnumerable<GetAllGeneratedVoucherCodesResponseDto>> GetAllGeneratedVoucherCodes(GetAllGeneratedVoucherCodesDto dto, CancellationToken stopToken = default);

    ValueTask<(int forward, int expectedResult, int voucherId, int voucherGeneratedCodeId)>
        AddVoucherByAgency(AddVoucherByAgencyDto dto, CancellationToken stopToken = default);

    ValueTask<int> RollbackAddVoucherByAgency(RollbackAddVoucherByAgencyDto dto, CancellationToken stopToken = default);
    ValueTask<IEnumerable<PackageDto>> GetAllActivePackagesAsync(CancellationToken stopToken = default);
    IEnumerable<PackageDto> GetAllActivePackages(CancellationToken stopToken = default);
    ValueTask<IEnumerable<PackageDto>> FilterActivePackages(string? search = null, CancellationToken stopToken = default);

}

//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
public class GaginoService : IGaginoService
{
    private readonly IDbConnection _connection;
    private readonly PackageCacheSettings _packageCacheSettings;
    private readonly ICacheManagerService _cacheManager;

    public GaginoService(
        IDbConnection connection,
        ICacheManagerService cacheManager,
        IOptions<PackageCacheSettings> packageCacheSettings)
    {
        _connection = connection;
        _cacheManager = cacheManager;
        _packageCacheSettings = packageCacheSettings.Value;
    }

    public async ValueTask<IEnumerable<GetAllUsersUsedVoucherCodeResponseDto>> GetAllUsersUsedVoucherCodes(GetAllUsersUsedVoucherCodesDto dto, CancellationToken stopToken = default)
    {
        stopToken.ThrowIfCancellationRequested();

        var gajinoUsers = await _connection.QueryAsync<GetAllUsersUsedVoucherCodeResponseDto>(
             sql: GajionoTsql.SP_GetAllUsersUsedVoucherCodes
             , param: new
             {
                 @cityCode = dto.CityCode,
                 @voucherCode = dto.VoucherCode,
                 @startDate = dto.StartDate,
                 @pageNumber = dto.PageNumber,
                 @pageSize = dto.PageSize
             });

        return gajinoUsers ?? Enumerable.Empty<GetAllUsersUsedVoucherCodeResponseDto>();
    }

    public async ValueTask<IEnumerable<GetAllGeneratedVoucherCodesResponseDto>> GetAllGeneratedVoucherCodes(GetAllGeneratedVoucherCodesDto dto
        , CancellationToken stopToken = default)
    {
        stopToken.ThrowIfCancellationRequested();

        var generatedVouchers = await _connection.QueryAsync<GetAllGeneratedVoucherCodesResponseDto>(
            sql: GajionoTsql.SP_GetAllGeneratedVoucherCodes
            , param: new
            {
                @cityCode = dto.CityCode,
                @startDate = dto.StartDate,
                @pageNumber = dto.PageNumber,
                @pageSize = dto.PageSize
            });

        return generatedVouchers ?? Enumerable.Empty<GetAllGeneratedVoucherCodesResponseDto>();
    }

    public async ValueTask<(int forward, int expectedResult, int voucherId, int voucherGeneratedCodeId)> AddVoucherByAgency(AddVoucherByAgencyDto dto, CancellationToken stopToken = default)
    {
        stopToken.ThrowIfCancellationRequested();
        var res = await _connection.QueryFirstOrDefaultAsync<(int, int, int, int)>(GajionoTsql.SP_AddVoucherByAgency, dto);
        return res;
    }

    public async ValueTask<int> RollbackAddVoucherByAgency(RollbackAddVoucherByAgencyDto dto, CancellationToken stopToken = default)
    {
        stopToken.ThrowIfCancellationRequested();
        var res = await _connection.ExecuteScalarAsync<int>(GajionoTsql.SP_RoolbackAddVoucherByAgency, dto);
        return res;
    }

    public async ValueTask<IEnumerable<PackageDto>> GetAllActivePackagesAsync(CancellationToken stopToken = default)
    {
        stopToken.ThrowIfCancellationRequested();
        IEnumerable<PackageDto>? packages = null;

        //get all packages from folder ActivePackages ---------> key is ActivePackages:*
        packages = await _cacheManager.GetAsync<IEnumerable<PackageDto>>($"{_packageCacheSettings.FolderName}");

        if (packages != null && packages.Any()) return packages;

        packages = await _connection.QueryAsync<PackageDto>(GajionoTsql.SP_GetAllActivePackages);


        await _cacheManager.SetAsync("",
            $"{_packageCacheSettings.FolderName}",
            packages,
            TimeSpan.FromDays(_packageCacheSettings.DayExpirationTime)
            );

        return packages ?? Enumerable.Empty<PackageDto>();

    }


    public IEnumerable<PackageDto> GetAllActivePackages(CancellationToken stopToken = default)
    {
        stopToken.ThrowIfCancellationRequested();
        IEnumerable<PackageDto>? packages = null;

        //get all packages from folder ActivePackages ---------> key is ActivePackages:*
        packages = _cacheManager.Get<IEnumerable<PackageDto>>($"{_packageCacheSettings.FolderName}");

        if (packages != null && packages.Any()) return packages;

        packages = _connection.Query<PackageDto>(GajionoTsql.SP_GetAllActivePackages);


        _cacheManager.Set("",
           $"{_packageCacheSettings.FolderName}",
           packages,
           TimeSpan.FromDays(_packageCacheSettings.DayExpirationTime)
       );

        return packages ?? Enumerable.Empty<PackageDto>();

    }

    public async ValueTask<IEnumerable<PackageDto>> FilterActivePackages(string? search = null, CancellationToken stopToken = default)
    {
        var packages = await GetAllActivePackagesAsync(stopToken);
        if (string.IsNullOrWhiteSpace(search?.Trim())) return packages;
        var filtered = packages.Where(w => w.Title.Contains(search)).ToList();
        return filtered;
    }


}

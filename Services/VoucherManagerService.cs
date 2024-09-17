using AutoMapper;
using GajinoAgencies.Data;
using GajinoAgencies.Dtos;
using GajinoAgencies.Models;
using GajinoAgencies.Settings;
using Microsoft.EntityFrameworkCore;

namespace GajinoAgencies.Services;


public interface IVoucherManagerService
{
    ValueTask<bool> AddNewVoucher(AddNewVoucherDto dto, CancellationToken stopToken = default);
    ValueTask<IEnumerable<GetAllGeneratedVoucherCodesResponseDto>> GetAllGeneratedVoucherCodes(GetAllGeneratedVoucherCodesDto dto, CancellationToken stopToken = default);
    ValueTask<IEnumerable<GetAllUsersUsedVoucherCodeResponseDto>> GetAllUsersUsedVoucherCodes(GetAllUsersUsedVoucherCodesDto dto, CancellationToken stopToken = default);

}
public class VoucherManagerService : IVoucherManagerService
{
    private readonly AgencyDbContext _ctx;
    private readonly IGaginoService _gagino;
    private readonly IMapper _mapper;

    public VoucherManagerService(AgencyDbContext ctx, IMapper mapper, IGaginoService gagino)
    {
        _ctx = ctx;
        _mapper = mapper;
        _gagino = gagino;
    }


    public async ValueTask<bool> AddNewVoucher(AddNewVoucherDto dto, CancellationToken stopToken = default)
    {
        var dbVoucherId = 0;
        var dbVoucherGeneratedCodeId = 0;
        var res = false;
        
        var oldVoucherCodes = await _ctx.Vouchers.Where(w => w.AgencyId == dto.AgencyId).Select(s => s.Code).Distinct().ToListAsync(stopToken);
        var voucherCode = GenerateVoucher(dto.CityCode!);

        while (oldVoucherCodes.Contains(voucherCode))
        {
            voucherCode = GenerateVoucher(dto.CityCode!);
        }

        dto.Code = voucherCode;

        var newVoucher = _mapper.Map<Voucher>(dto);

        var executionStrategy = _ctx.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(
            async () =>
            {
                await using var tran = await _ctx.Database.BeginTransactionAsync(stopToken);
                try
                {
                    _ctx.Vouchers.Add(newVoucher);
                    res = await _ctx.SaveChangesAsync(stopToken) > 0;

                    var spResponse = await _gagino.AddVoucherByAgency(dto, stopToken);

                    dbVoucherId = spResponse.voucherId;
                    dbVoucherGeneratedCodeId = spResponse.voucherGeneratedCodeId;

                    if (res && spResponse.forward == spResponse.expectedResult)
                    {
                        await tran.CommitAsync(stopToken);
                    }
                    else
                    {
                        await Compensation(dbVoucherId, dbVoucherGeneratedCodeId);
                        await tran.RollbackAsync(stopToken);
                    }
                }
                catch (Exception ex)
                {
                    await Compensation(dbVoucherId, dbVoucherGeneratedCodeId);
                    await tran.RollbackAsync(stopToken);
                }
            });
        return res;
    }

    public async ValueTask<IEnumerable<GetAllGeneratedVoucherCodesResponseDto>> GetAllGeneratedVoucherCodes(GetAllGeneratedVoucherCodesDto dto, CancellationToken stopToken = default)
    {
        stopToken.ThrowIfCancellationRequested();

        var data = await _gagino.GetAllGeneratedVoucherCodes(dto, stopToken);

        //var usedVoucherCodes = data?.Select(s => s.VoucherCode).Distinct().ToList() ?? Enumerable.Empty<string>();

        //var vouchersExpirationList = await _ctx.Vouchers
        //    .Where(w => w.AgencyId == dto.AgencyId && usedVoucherCodes.Contains(w.Code))
        //    .Select(s => new VoucherExpirationDto(s.Code, s.Expiration))
        //    .Distinct()
        //    .ToListAsync(stopToken);

        ////update voucher code expiration date
        //foreach (var item in vouchersExpirationList)
        //{
        //    data
        //        ?.Where(w => w.VoucherCode == item.Code)
        //        ?.ToList()
        //        ?.ForEach(x => x.ExpireDate = item.Expiration);
        //}
        return data ?? Enumerable.Empty<GetAllGeneratedVoucherCodesResponseDto>();
    }


    public async ValueTask<IEnumerable<GetAllUsersUsedVoucherCodeResponseDto>> GetAllUsersUsedVoucherCodes(GetAllUsersUsedVoucherCodesDto dto, CancellationToken stopToken = default)
    {
        stopToken.ThrowIfCancellationRequested();

        var data = await _gagino.GetAllUsersUsedVoucherCodes(dto, stopToken);

        //var usedVoucherCodes = data?.Select(s => s.VoucherCode).Distinct().ToList() ?? Enumerable.Empty<string>();

        //var vouchersExpirationList = await _ctx.Vouchers
        //    .Where(w => w.AgencyId == dto.AgencyId && usedVoucherCodes.Contains(w.Code))
        //    .Select(s => new VoucherExpirationDto(s.Code, s.Expiration))
        //    .Distinct()
        //    .ToListAsync(stopToken);

        ////update voucher code expiration date
        //foreach (var item in vouchersExpirationList)
        //{
        //    data
        //        ?.Where(w => w.VoucherCode == item.Code)
        //        ?.ToList()
        //        ?.ForEach(x => x.ExpireDate = item.Expiration);
        //}
        return data ?? Enumerable.Empty<GetAllUsersUsedVoucherCodeResponseDto>();

    }



    private async Task Compensation(int voucherId, int voucherGeneratedCodeId)
    {
        var res = await _gagino.RollbackAddVoucherByAgency(
            new RollbackAddVoucherByAgencyDto(voucherId, voucherGeneratedCodeId));
    }

    private static string GenerateVoucher(string cityCode)
    {
        var keys = AppConstants.VoucherCodeRandomString.ToCharArray();
        var random = new Random();
        var generatedCode = Enumerable
            .Range(1, 4)
            .Select(k => keys[random.Next(0, keys.Length - 1)])
            .Aggregate("", (e, c) => e + c);

        var code = $"{cityCode}{generatedCode}";
        return code;
    }


    private record VoucherExpirationDto(string Code, DateTime Expiration);

}

using System.Data;
using System.Security.Claims;
using AutoMapper;
using GajinoAgencies.Data;
using GajinoAgencies.Dtos;
using GajinoAgencies.Models;
using GajinoAgencies.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GajinoAgencies.Services;

public interface IUserAccountService
{
    ValueTask<string> Login(LoginRequestDto dto, CancellationToken stopToken = default);
    ValueTask<bool> Register(AddAgentRequestDto dto, CancellationToken stopToken = default);
    ValueTask<bool> Logout(string userId, CancellationToken stopToken = default);
    ValueTask<bool> ChangePassword(ChangePasswordDto dto, CancellationToken stopToken = default);
    ValueTask<bool> ResetPassword(ResetPasswordDto dto, CancellationToken stopToken = default);
    ValueTask<Agency> GetUserByMobileNumber(string mobile, CancellationToken stopToken = default);

    (int userId, string cityCode, string mobile) GetUserInfoFromToken(IEnumerable<Claim> claims);
}



public class UserAccountService : IUserAccountService
{

    private readonly AgencyDbContext _ctx;
    private readonly IMapper _mapper;
    private readonly ITokenManagerService _tokenManager;
    private readonly IPasswordManagerService _passwordManager;
    private readonly ICacheManagerService _redisCacheManager;
    private readonly IOtpService _otp;
    private readonly JwtSettings _jwtSettings;

    public UserAccountService(AgencyDbContext ctx,
        IMapper mapper,
        ITokenManagerService tokenManager,
        IPasswordManagerService passwordManager,
        ICacheManagerService redisCacheManager,
        IOtpService otp,
        IOptions<JwtSettings> jwtSettings
        )
    {
        _ctx = ctx;
        _mapper = mapper;
        _tokenManager = tokenManager;
        _passwordManager = passwordManager;
        _redisCacheManager = redisCacheManager;
        _otp = otp;
        _jwtSettings = jwtSettings.Value;
    }

    public async ValueTask<string> Login(LoginRequestDto dto, CancellationToken stopToken = default)
    {
        var user = await _ctx.Agencies
            .Include(x => x.Location)
            .FirstOrDefaultAsync(x => x.Mobile == dto.Username, stopToken);

        if (user == null || user.Password != dto.Password)
            //if (user == null || !_passwordManager.VerifyPassword(dto.Password, user.Password, user.Salt))
            throw new KeyNotFoundException($"Invalid Username or password");

        var token = _tokenManager.GenerateToken(new TokenRequestDto(user.Id, user.Mobile, user?.Location?.CityCode ?? ""));

        await _redisCacheManager.SetAsync($"{_jwtSettings.FolderName!}:" ,user!.Id.ToString(), token);

        return token;
    }

    public async ValueTask<bool> Register(AddAgentRequestDto dto, CancellationToken stopToken = default)
    {

        var agency = _mapper.Map<Agency>(dto);
        agency.Salt = "123456789";
        if (_ctx.Agencies.Any(x => x.Mobile == dto.Mobile))
        {
            throw new DuplicateNameException("Username already exist");
        }
        _ctx.Agencies.Add(agency);
        var res = await _ctx.SaveChangesAsync(stopToken);
        return res > 0;
    }

    public async ValueTask<bool> Logout(string userId, CancellationToken stopToken = default)
    {
        var userIdKey = $"{_jwtSettings.FolderName}:{userId}";
        return await _redisCacheManager.RemoveAsync(userIdKey);
    }

    public async ValueTask<bool> ChangePassword(ChangePasswordDto dto, CancellationToken stopToken = default)
    {
        var user = await _ctx.Agencies.FindAsync(dto.UserId);

        if (user == null || user.Password != dto.OldPassword)
            throw new KeyNotFoundException($"Invalid User");

        user.Password = dto.NewPassword;

        _ctx.Agencies.Update(user);
        return await _ctx.SaveChangesAsync(stopToken) > 0;
    }

    public async ValueTask<bool> ResetPassword(ResetPasswordDto dto, CancellationToken stopToken = default)
    {
        var user = await GetUserByMobileNumber(dto.Mobile, stopToken);
        if (user is null || !await _otp.IsValid(dto.Mobile, dto.OtpCode))
        {
            throw new KeyNotFoundException($"Invalid mobile number or Otp code");
        }
        var (hashedPassword, salt) = _passwordManager.HashPassword(dto.Password);
        user.Password = dto.Password; //hashedPassword;
        //user.Salt = salt;
        _ctx.Agencies.Update(user);
        return await _ctx.SaveChangesAsync(stopToken) > 0;

    }

    public async ValueTask<Agency> GetUserByMobileNumber(string mobile, CancellationToken stopToken = default)
    {
        var user = await _ctx.Agencies.FirstOrDefaultAsync(x => x.Mobile == mobile, stopToken);
        if (user == null)
            throw new KeyNotFoundException($"Invalid User, User not found with mobile number {mobile}");
        return user;
    }


    public (int userId, string cityCode, string mobile) GetUserInfoFromToken(IEnumerable<Claim> claims)
    {
        var enumerable = claims as Claim[] ?? claims.ToArray();
        var res = (
                int.Parse(enumerable.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value),
                enumerable.FirstOrDefault(c => c.Type == "cityCode")!.Value,
                enumerable.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)!.Value
        );
        return res;
    }




}

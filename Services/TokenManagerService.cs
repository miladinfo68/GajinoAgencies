using GajinoAgencies.Dtos;
using GajinoAgencies.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GajinoAgencies.Services;


public interface ITokenManagerService
{
    string GenerateToken(TokenRequestDto dto, CancellationToken stopToken = default);
}

//############################
public class TokenManagerService : ITokenManagerService
{
    private readonly JwtSettings _jwtSettings;
    public TokenManagerService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }
    public string GenerateToken(TokenRequestDto dto, CancellationToken stopToken = default)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, dto.UserId.ToString()),
            new Claim(ClaimTypes.MobilePhone, dto.Username),
            new Claim("cityCode", dto.CityCode),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            //new Claim(JwtRegisteredClaimNames.Sub, dto.UserId.ToString())
          
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtSettings.SecretKey!));
        
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var securityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.MinuteExpirationTime),
            signingCredentials: cred
        );
        
        var jwtToken = new JwtSecurityTokenHandler().WriteToken(securityToken);
        
        return jwtToken;

    }
}

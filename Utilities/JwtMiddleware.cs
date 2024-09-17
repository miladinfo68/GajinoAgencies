using Microsoft.AspNetCore.Authorization;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GajinoAgencies.Services;
using GajinoAgencies.Settings;
using Microsoft.Extensions.Options;

namespace GajinoAgencies.Utilities;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public JwtMiddleware(RequestDelegate next,
        IOptions<JwtSettings> jwtSettings,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        // Check if the endpoint has the AllowAnonymous attribute
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            await _next(context);
            return;
        }

        var token = context.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "") ?? "";

        if (string.IsNullOrEmpty(token))
        {
            var response = ApiResponse.ResultMessage<string>(
                  data: null,
                  status: HttpStatusCode.Unauthorized,
                  message: "Unauthorized: No token provided."
              );

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            // Serialize the response and write it to the response body
            await context.Response.WriteAsJsonAsync(response);
            return;
        }


        // Validate the token
        var tokenValidationResult = CheckTokenValidation(token);
        var redisCacheManager = context.RequestServices.GetRequiredService<ICacheManagerService>();
        var existToken = await redisCacheManager.GetAsync<string>($"{_jwtSettings.FolderName}:{tokenValidationResult.UserId}");
        if (!tokenValidationResult.isValid || string.IsNullOrEmpty(existToken))
        {
            var response = ApiResponse.ResultMessage<string>(
                data: null,
                status: HttpStatusCode.Unauthorized,
                message: "Unauthorized: Invalid or expired token."
            );

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            // Serialize the response and write it to the response body
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        if (tokenValidationResult.claimsPrincipal != null)
        {
            context.User = tokenValidationResult.claimsPrincipal;
        }

        await _next(context);
    }

    private (bool isValid, string UserId, ClaimsPrincipal? claimsPrincipal) CheckTokenValidation(string token)
    {
        ClaimsPrincipal? claimsPrincipal = null;
        var isValid = true;
        var userId = "-1";
        try
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey!))
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            if (tokenHandler.CanReadToken(token))
            {
                claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var expClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "exp");
                    userId = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? "-1";
                    if (expClaim != null)
                    {
                        // Convert the expiration claim from Unix time to DateTime
                        var expUnixTime = long.Parse(expClaim.Value);
                        var expirationDate = DateTimeOffset.FromUnixTimeSeconds(expUnixTime).UtcDateTime;

                        // Check if the token is expired
                        isValid = expirationDate >= DateTime.UtcNow;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            isValid = false;
            _logger.LogError(ex, "JWT token validation failed");
        }
        return (isValid, userId, claimsPrincipal);
    }

}
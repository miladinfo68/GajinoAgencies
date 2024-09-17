using GajinoAgencies.Services;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Security.Claims;

namespace GajinoAgencies.Utilities;

public class AdminRequirement : IAuthorizationRequirement { }


public class AdminAuthorizationHandler : AuthorizationHandler<AdminRequirement>
{
    private readonly IUserAccountService _userAccount;

    public AdminAuthorizationHandler(IUserAccountService userAccount)
    {
        _userAccount = userAccount;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
    {
        var userMobileClaim = context.User.FindFirst(ClaimTypes.MobilePhone); // or the claim you use for User ID

        if (userMobileClaim != null)
        {
            var user = await _userAccount.GetUserByMobileNumber(userMobileClaim.Value);
            if (user is { IsAdmin: true })
            {
                context.Succeed(requirement);
            }
            else
            {
                throw new ForbiddenAccessException("Access denied: You do not have permission to access this resource.");
                //context.Fail();
            }
        }
        else
        {
            context.Fail();
        }
    }
}


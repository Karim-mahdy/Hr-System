using Hr.Application.Common.Filter;
using Hr.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;


public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirment>
{
    private readonly IConfiguration configuration;
    private readonly UserManager<ApplicationUser> userManager;
  

    public PermissionAuthorizationHandler(IConfiguration configuration)
    {
        this.configuration = configuration;
     
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirment requirement)
    {
        if (context.User == null)      
            return;
      
        // Accessing a setting from the configuration file
        string validIssuer = configuration["JWT:ValidIssuer"];
        var canAccess = context.User.Claims.Any(c => c.Type == "Permission" && c.Value == requirement.Permission && c.Issuer == validIssuer);

        if (canAccess)
        {
            context.Succeed(requirement);
            return;
        }
    }
}

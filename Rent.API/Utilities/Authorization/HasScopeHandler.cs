using Microsoft.AspNetCore.Authorization;

namespace Rent.API.Utilities.Authorization;

public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        HasScopeRequirement requirement
        )
    {
        if (!context.User.HasClaim(c => c.Type == "scope" && c.Issuer == requirement.Issuer))
        {
            var scopes = context.User
              .FindAll(c => c.Type == "scope" && c.Issuer == requirement.Issuer);

            if (scopes.Any(s => s.Value == requirement.Scope))
                context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
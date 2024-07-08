using Microsoft.AspNetCore.Authorization;

namespace Rent.API.Utilities.Authorization;

public class HasScopeRequirement(string? scope, string? issuer) : IAuthorizationRequirement
{
    public string Scope { get; set; } = scope ?? throw new ArgumentNullException(nameof(scope));
    public string Issuer { get; set; } = issuer ?? throw new ArgumentNullException(nameof(issuer));
}
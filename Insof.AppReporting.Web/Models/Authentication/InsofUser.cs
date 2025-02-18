using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Insof.AppReporting.Web.Models.Authentication;

public sealed class InsofUser
{
    internal ClaimsPrincipal ClaimsPrincipal
    {
        get
        {
            var claims = Roles
                .Select(r => new Claim(ClaimTypes.Role, r.Name))
                .ToList();
            claims.Add(new Claim(ClaimTypes.Sid, Username));
            claims.Add(new Claim(ClaimTypes.Name, Name));
            return new ClaimsPrincipal(new ClaimsIdentity(claims, nameof(InsofUser)));
        }
    }

    public required int UserId { get; init; }
    public required string Username { get; init; }
    public required string Name { get; init; }
    public required List<Role> Roles { get; init; }
    public required string Secret { get; init; }

    public static InsofUser Empty { get; } = new()
    {
        UserId = 0,
        Username = string.Empty,
        Name = "Empty",
        Roles = [],
        Secret = string.Empty
    };
}
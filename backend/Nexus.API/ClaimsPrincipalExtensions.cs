using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Nexus.API;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var subject = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("O token não contém o claim de identificação do usuário.");

        return Guid.Parse(subject);
    }
}

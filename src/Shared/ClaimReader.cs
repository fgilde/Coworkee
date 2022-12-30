using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Coworkee.Shared;

public static class ClaimReader
{
    public static IEnumerable<Claim> ReadClaimsFromJwt(string jwt)
    {
        return new JwtSecurityTokenHandler().ReadJwtToken(jwt).Claims;
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Marketplace.Web.MVC.Services.Auth;

public class JwtDecoder
{
    public ClaimsPrincipal Decode(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(accessToken);

        var claims = new List<Claim>(jwt.Claims);

        // Normalize role claim
        var roleClaim = claims.FirstOrDefault(c =>
            c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" ||
            c.Type == "role");

        if (roleClaim != null && roleClaim.Type != ClaimTypes.Role)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleClaim.Value));
        }

        // Normalize NameIdentifier
        var subClaim = claims.FirstOrDefault(c => c.Type == "sub" || c.Type == JwtRegisteredClaimNames.Sub);
        if (subClaim != null)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, subClaim.Value));
        }

        var emailClaim = claims.FirstOrDefault(c => c.Type == "email" || c.Type == JwtRegisteredClaimNames.Email);
        if (emailClaim != null)
        {
            claims.Add(new Claim(ClaimTypes.Email, emailClaim.Value));
        }

        var nameClaim = claims.FirstOrDefault(c => c.Type == "name" || c.Type == JwtRegisteredClaimNames.Name);
        if (nameClaim != null)
        {
            claims.Add(new Claim(ClaimTypes.Name, nameClaim.Value));
        }

        var identity = new ClaimsIdentity(claims, "Cookie");
        return new ClaimsPrincipal(identity);
    }
}

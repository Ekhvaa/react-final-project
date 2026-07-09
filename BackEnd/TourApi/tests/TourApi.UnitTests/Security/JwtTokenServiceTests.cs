using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using TourApi.Common;
using TourApi.Constants;
using TourApi.Services;
using Xunit;

namespace TourApi.UnitTests.Security;

public sealed class JwtTokenServiceTests
{
    [Fact]
    public void GenerateToken_ShouldIncludeUserClaimsAndRole()
    {
        var service = new JwtTokenService(Options.Create(new JwtOptions
        {
            Issuer = "TourApi.Tests",
            Audience = "TourApi.Tests.Client",
            Key = "THIS_IS_A_LONG_TEST_SECRET_KEY_FOR_JWT_SIGNING_123456",
            ExpiryMinutes = 30
        }));

        var (token, expiresAtUtc) = service.GenerateToken(5, "lado", ApplicationRoles.Tourist);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal("TourApi.Tests", jwt.Issuer);
        Assert.Contains(jwt.Audiences, audience => audience == "TourApi.Tests.Client");
        Assert.Contains(jwt.Claims, claim => claim.Type == JwtRegisteredClaimNames.Sub && claim.Value == "5");
        Assert.Contains(jwt.Claims, claim => (claim.Type == ClaimTypes.NameIdentifier || claim.Type == "nameid") && claim.Value == "5");
        Assert.Contains(jwt.Claims, claim => (claim.Type == ClaimTypes.Name || claim.Type == "unique_name") && claim.Value == "lado");
        Assert.Contains(jwt.Claims, claim => (claim.Type == ClaimTypes.Role || claim.Type == "role") && claim.Value == ApplicationRoles.Tourist);
        Assert.True(expiresAtUtc > DateTime.UtcNow);
    }
}

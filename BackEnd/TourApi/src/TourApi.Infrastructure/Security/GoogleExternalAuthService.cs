using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using TourApi.Common;
using TourApi.DTOs.Auth;
using TourApi.Services;

namespace TourApi.Infrastructure.Security;

public sealed class GoogleExternalAuthService : IGoogleExternalAuthService
{
    private readonly GoogleAuthOptions _options;

    public GoogleExternalAuthService(IOptions<GoogleAuthOptions> options)
    {
        _options = options.Value;
    }

    public async Task<ExternalUserInfo> GetUserInfoAsync(
        string idToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idToken))
            throw new UnauthorizedAccessException("Google id token is required.");

        if (string.IsNullOrWhiteSpace(_options.ClientId))
            throw new InvalidOperationException("Google client id is not configured.");

        var payload = await GoogleJsonWebSignature.ValidateAsync(
            idToken,
            new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _options.ClientId }
            });

        if (string.IsNullOrWhiteSpace(payload.Subject))
            throw new UnauthorizedAccessException("Invalid Google token.");

        if (!payload.EmailVerified)
            throw new UnauthorizedAccessException("Google email is not verified.");

        return new ExternalUserInfo
        {
            Provider = "Google",
            ProviderUserId = payload.Subject,
            Email = payload.Email ?? string.Empty,
            FirstName = payload.GivenName ?? string.Empty,
            LastName = payload.FamilyName ?? string.Empty
        };
    }
}

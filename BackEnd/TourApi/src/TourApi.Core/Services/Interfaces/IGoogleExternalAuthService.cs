using TourApi.DTOs.Auth;

namespace TourApi.Services;

public interface IGoogleExternalAuthService
{
    Task<ExternalUserInfo> GetUserInfoAsync(string idToken, CancellationToken cancellationToken = default);
}

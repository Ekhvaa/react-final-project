using TourApi.DTOs.Auth;

namespace TourApi.Services;

public interface IFacebookExternalAuthService
{
    Task<ExternalUserInfo> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken = default);
}

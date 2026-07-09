using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using TourApi.Common;
using TourApi.DTOs.Auth;
using TourApi.Services;

namespace TourApi.Infrastructure.Security;

public sealed class FacebookExternalAuthService : IFacebookExternalAuthService
{
    private static readonly HttpClient HttpClient = new()
    {
        BaseAddress = new Uri("https://graph.facebook.com/")
    };

    private readonly FacebookAuthOptions _options;

    public FacebookExternalAuthService(IOptions<FacebookAuthOptions> options)
    {
        _options = options.Value;
    }

    public async Task<ExternalUserInfo> GetUserInfoAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new UnauthorizedAccessException("Facebook access token is required.");

        if (string.IsNullOrWhiteSpace(_options.AppId) || string.IsNullOrWhiteSpace(_options.AppSecret))
            throw new InvalidOperationException("Facebook app id/app secret are not configured.");

        var debug = await DebugTokenAsync(accessToken, cancellationToken);
        if (debug?.Data?.IsValid != true || debug.Data.AppId != _options.AppId)
            throw new UnauthorizedAccessException("Invalid Facebook token.");

        var appSecretProof = CreateAppSecretProof(accessToken, _options.AppSecret);
        var url =
            $"me?fields=id,email,first_name,last_name" +
            $"&access_token={Uri.EscapeDataString(accessToken)}" +
            $"&appsecret_proof={appSecretProof}";

        var facebookUser = await HttpClient.GetFromJsonAsync<FacebookUserResponse>(url, cancellationToken);

        if (facebookUser is null || string.IsNullOrWhiteSpace(facebookUser.Id))
            throw new UnauthorizedAccessException("Invalid Facebook token.");

        if (string.IsNullOrWhiteSpace(facebookUser.Email))
            throw new InvalidOperationException("Facebook did not return an email address. Ask for the email permission on the frontend.");

        return new ExternalUserInfo
        {
            Provider = "Facebook",
            ProviderUserId = facebookUser.Id,
            Email = facebookUser.Email,
            FirstName = facebookUser.FirstName ?? string.Empty,
            LastName = facebookUser.LastName ?? string.Empty
        };
    }

    private async Task<FacebookDebugTokenResponse?> DebugTokenAsync(
        string accessToken,
        CancellationToken cancellationToken)
    {
        var appAccessToken = $"{_options.AppId}|{_options.AppSecret}";
        var url =
            $"debug_token?input_token={Uri.EscapeDataString(accessToken)}" +
            $"&access_token={Uri.EscapeDataString(appAccessToken)}";

        return await HttpClient.GetFromJsonAsync<FacebookDebugTokenResponse>(url, cancellationToken);
    }

    private static string CreateAppSecretProof(string accessToken, string appSecret)
    {
        var key = Encoding.UTF8.GetBytes(appSecret);
        var data = Encoding.UTF8.GetBytes(accessToken);
        var hash = HMACSHA256.HashData(key, data);

        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private sealed class FacebookDebugTokenResponse
    {
        [JsonPropertyName("data")]
        public FacebookDebugTokenData? Data { get; set; }
    }

    private sealed class FacebookDebugTokenData
    {
        [JsonPropertyName("app_id")]
        public string? AppId { get; set; }

        [JsonPropertyName("is_valid")]
        public bool IsValid { get; set; }
    }

    private sealed class FacebookUserResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }
    }
}

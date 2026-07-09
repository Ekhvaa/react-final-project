using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using TourApi.Common;
using TourApi.Constants;
using TourApi.DTOs.Auth;
using TourApi.Exceptions;
using TourApi.Factories;
using TourApi.Models;
using TourApi.Repositories;

namespace TourApi.Services;

public class AuthService : IAuthService
{
    private const int MaxEmailLength = 50;
    private const int MaxUsernameLength = 30;
    private const int UsernameBaseLength = 20;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUserFactory _userFactory;
    private readonly IGoogleExternalAuthService _googleExternalAuthService;
    private readonly IFacebookExternalAuthService _facebookExternalAuthService;
    private readonly IEmailSender _emailSender;
    private readonly RefreshTokenOptions _refreshTokenOptions;
    private readonly EmailOptions _emailOptions;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IUserFactory userFactory,
        IGoogleExternalAuthService googleExternalAuthService,
        IFacebookExternalAuthService facebookExternalAuthService,
        IEmailSender emailSender,
        IOptions<RefreshTokenOptions> refreshTokenOptions,
        IOptions<EmailOptions> emailOptions)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _userFactory = userFactory;
        _googleExternalAuthService = googleExternalAuthService;
        _facebookExternalAuthService = facebookExternalAuthService;
        _emailSender = emailSender;
        _refreshTokenOptions = refreshTokenOptions.Value;
        _emailOptions = emailOptions.Value;
    }

    public async Task<AuthResponse> RegisterAsync(
        RegisterRequest request,
        string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        ValidatePersonalInfo(request.DateOfBirth, request.Gender);

        if (await _unitOfWork.Users.IsUsernameTakenAsync(request.Username, cancellationToken))
            throw new DuplicateResourceException("Username is already taken.");

        if (await _unitOfWork.Users.GetActiveByEmailAsync(request.Email, cancellationToken) is not null)
            throw new DuplicateResourceException("Email is already taken.");

        if (await _unitOfWork.Tourists.ExistsAsync(t => t.NationalId == request.NationalId && !t.IsDeleted, cancellationToken)
            || await _unitOfWork.Employees.ExistsAsync(e => e.NationalId == request.NationalId && !e.IsDeleted, cancellationToken))
            throw new DuplicateResourceException("National ID is already taken.");

        var (user, tourist) = _userFactory.CreateTouristUser(request, _passwordHasher.Hash(request.Password));

        _unitOfWork.Users.Add(user);
        _unitOfWork.Tourists.Add(tourist);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await CreateAndSendEmailConfirmationAsync(user.Id, request.Email, cancellationToken);

        return await CreateAuthResponseAsync(user, ApplicationRoles.Tourist, ipAddress, cancellationToken);
    }

    public async Task<AuthResponse?> LoginAsync(
        LoginRequest request,
        string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetActiveByUsernameAsync(request.Username, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return null;

        if (_emailOptions.RequireConfirmedEmail && !user.EmailConfirmed)
            throw new UnauthorizedAccessException("Email address is not confirmed.");

        var role = await ResolveRoleAsync(user.Id, cancellationToken);
        return role is null ? null : await CreateAuthResponseAsync(user, role, ipAddress, cancellationToken);
    }

    public async Task<AuthResponse> LoginWithGoogleAsync(
        GoogleLoginRequest request,
        string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        var externalUser = await _googleExternalAuthService.GetUserInfoAsync(request.IdToken, cancellationToken);
        return await LoginOrRegisterExternalUserAsync(externalUser, ipAddress, cancellationToken);
    }

    public async Task<AuthResponse> LoginWithFacebookAsync(
        FacebookLoginRequest request,
        string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        var externalUser = await _facebookExternalAuthService.GetUserInfoAsync(request.AccessToken, cancellationToken);
        return await LoginOrRegisterExternalUserAsync(externalUser, ipAddress, cancellationToken);
    }

    public async Task<AuthResponse> RefreshAsync(
        RefreshTokenRequest request,
        string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(request.RefreshToken);
        var storedToken = await _unitOfWork.RefreshTokens.GetActiveByHashAsync(tokenHash, cancellationToken);

        if (storedToken is null || !storedToken.IsActive || storedToken.User.IsDeleted)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        var role = await ResolveRoleAsync(storedToken.UserId, cancellationToken);
        if (role is null)
            throw new UnauthorizedAccessException("User does not have a valid role.");

        var replacement = CreateRefreshToken(storedToken.UserId, ipAddress);
        storedToken.RevokedAtUtc = DateTime.UtcNow;
        storedToken.RevokedByIp = ipAddress;
        storedToken.ReplacedByTokenHash = replacement.Entity.TokenHash;

        _unitOfWork.RefreshTokens.Add(replacement.Entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreateAuthResponse(storedToken.User, role, replacement.PlainToken, replacement.Entity.ExpiresAtUtc);
    }

    public async Task LogoutAsync(
        RevokeTokenRequest request,
        string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(request.RefreshToken);
        var storedToken = await _unitOfWork.RefreshTokens.GetActiveByHashAsync(tokenHash, cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
            return;

        storedToken.RevokedAtUtc = DateTime.UtcNow;
        storedToken.RevokedByIp = ipAddress;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(request.Token);
        var storedToken = await _unitOfWork.EmailConfirmationTokens.GetActiveByHashAsync(tokenHash, cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
            throw new InvalidOperationException("Email confirmation token is invalid or expired.");

        storedToken.UsedAtUtc = DateTime.UtcNow;
        storedToken.User.EmailConfirmed = true;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetActiveByEmailAsync(request.Email, cancellationToken);
        if (user is null)
            return;

        var plainToken = GenerateSecureToken();
        var token = new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = HashToken(plainToken),
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_emailOptions.PasswordResetTokenExpiryMinutes)
        };

        _unitOfWork.PasswordResetTokens.Add(token);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _emailSender.SendPasswordResetAsync(request.Email, plainToken, cancellationToken);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(request.Token);
        var storedToken = await _unitOfWork.PasswordResetTokens.GetActiveByHashAsync(tokenHash, cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
            throw new InvalidOperationException("Password reset token is invalid or expired.");

        storedToken.User.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        storedToken.User.EmailConfirmed = true;
        storedToken.UsedAtUtc = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ResendEmailConfirmationAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetActiveByEmailAsync(email, cancellationToken);
        if (user is null || user.EmailConfirmed)
            return;

        await CreateAndSendEmailConfirmationAsync(user.Id, email, cancellationToken);
    }

    private async Task<AuthResponse> LoginOrRegisterExternalUserAsync(
        ExternalUserInfo externalUser,
        string? ipAddress,
        CancellationToken cancellationToken)
    {
        ValidateExternalUserInfo(externalUser);

        var linkedLogin = await _unitOfWork.ExternalLogins.GetByProviderAsync(
            externalUser.Provider,
            externalUser.ProviderUserId,
            cancellationToken);

        if (linkedLogin is not null)
        {
            var linkedRole = await ResolveRoleAsync(linkedLogin.UserId, cancellationToken);
            if (linkedRole is null)
                throw new InvalidOperationException("External login is linked to a user without a valid role.");

            return await CreateAuthResponseAsync(linkedLogin.User, linkedRole, ipAddress, cancellationToken);
        }

        var existingUser = await _unitOfWork.Users.GetActiveByEmailAsync(externalUser.Email, cancellationToken);
        if (existingUser is not null)
        {
            var externalLogin = new ExternalLogin
            {
                UserId = existingUser.Id,
                Provider = externalUser.Provider,
                ProviderUserId = externalUser.ProviderUserId,
                Email = externalUser.Email
            };

            existingUser.EmailConfirmed = true;
            _unitOfWork.ExternalLogins.Add(externalLogin);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var existingRole = await ResolveRoleAsync(existingUser.Id, cancellationToken);
            if (existingRole is null)
                throw new InvalidOperationException("User does not have a valid role.");

            return await CreateAuthResponseAsync(existingUser, existingRole, ipAddress, cancellationToken);
        }

        var username = await GenerateUniqueUsernameAsync(externalUser.Email, cancellationToken);
        var unusablePasswordHash = _passwordHasher.Hash(Guid.NewGuid().ToString("N"));
        var nationalId = GenerateExternalNationalId();

        var (user, tourist, login) = _userFactory.CreateExternalTouristUser(
            externalUser,
            username,
            unusablePasswordHash,
            nationalId);

        _unitOfWork.Users.Add(user);
        _unitOfWork.Tourists.Add(tourist);
        _unitOfWork.ExternalLogins.Add(login);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await CreateAuthResponseAsync(user, ApplicationRoles.Tourist, ipAddress, cancellationToken);
    }

    private async Task CreateAndSendEmailConfirmationAsync(int userId, string email, CancellationToken cancellationToken)
    {
        var plainToken = GenerateSecureToken();
        var token = new EmailConfirmationToken
        {
            UserId = userId,
            TokenHash = HashToken(plainToken),
            ExpiresAtUtc = DateTime.UtcNow.AddHours(_emailOptions.ConfirmationTokenExpiryHours)
        };

        _unitOfWork.EmailConfirmationTokens.Add(token);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _emailSender.SendEmailConfirmationAsync(email, plainToken, cancellationToken);
    }

    private async Task<AuthResponse> CreateAuthResponseAsync(
        User user,
        string role,
        string? ipAddress,
        CancellationToken cancellationToken)
    {
        var refreshToken = CreateRefreshToken(user.Id, ipAddress);
        _unitOfWork.RefreshTokens.Add(refreshToken.Entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreateAuthResponse(user, role, refreshToken.PlainToken, refreshToken.Entity.ExpiresAtUtc);
    }

    private AuthResponse CreateAuthResponse(User user, string role, string refreshToken, DateTime refreshTokenExpiresAtUtc)
    {
        var (token, expires) = _jwtTokenService.GenerateToken(user.Id, user.Username, role);

        return new AuthResponse
        {
            Token = token,
            ExpiresAtUtc = expires,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc,
            UserId = user.Id,
            Username = user.Username,
            Role = role,
            EmailConfirmed = user.EmailConfirmed
        };
    }

    private (RefreshToken Entity, string PlainToken) CreateRefreshToken(int userId, string? ipAddress)
    {
        var plainToken = GenerateSecureToken();
        var token = new RefreshToken
        {
            UserId = userId,
            TokenHash = HashToken(plainToken),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_refreshTokenOptions.ExpiryDays),
            CreatedByIp = ipAddress
        };

        return (token, plainToken);
    }

    private static void ValidateExternalUserInfo(ExternalUserInfo externalUser)
    {
        if (string.IsNullOrWhiteSpace(externalUser.Provider))
            throw new InvalidOperationException("External provider is missing.");

        if (string.IsNullOrWhiteSpace(externalUser.ProviderUserId))
            throw new InvalidOperationException("External provider user id is missing.");

        if (string.IsNullOrWhiteSpace(externalUser.Email))
            throw new InvalidOperationException("External provider did not return an email address.");

        if (externalUser.Email.Length > MaxEmailLength)
            throw new InvalidOperationException($"Email address cannot be longer than {MaxEmailLength} characters.");
    }

    private static void ValidatePersonalInfo(DateTime dateOfBirth, char gender)
    {
        if (dateOfBirth.Date >= DateTime.UtcNow.Date)
            throw new InvalidOperationException("Date of birth must be in the past.");

        if (gender is not ('M' or 'F'))
            throw new InvalidOperationException("Gender must be 'M' or 'F'.");
    }

    private async Task<string> GenerateUniqueUsernameAsync(string email, CancellationToken cancellationToken)
    {
        var baseUsername = email.Split('@')[0];
        baseUsername = NormalizeUsername(baseUsername);

        if (string.IsNullOrWhiteSpace(baseUsername))
            baseUsername = "user";

        if (baseUsername.Length > UsernameBaseLength)
            baseUsername = baseUsername[..UsernameBaseLength];

        if (!await _unitOfWork.Users.IsUsernameTakenAsync(baseUsername, cancellationToken))
            return baseUsername;

        for (var attempt = 1; attempt <= 10; attempt++)
        {
            var suffix = Random.Shared.Next(1000, 9999).ToString();
            var candidateBaseLength = Math.Min(baseUsername.Length, MaxUsernameLength - suffix.Length - 1);
            var candidate = $"{baseUsername[..candidateBaseLength]}_{suffix}";

            if (!await _unitOfWork.Users.IsUsernameTakenAsync(candidate, cancellationToken))
                return candidate;
        }

        return $"user_{Guid.NewGuid():N}"[..MaxUsernameLength];
    }

    private static string NormalizeUsername(string value)
    {
        var builder = new StringBuilder(value.Length);

        foreach (var character in value.ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(character) || character is '_' or '.' or '-')
                builder.Append(character);
        }

        return builder.ToString();
    }

    private static string GenerateExternalNationalId()
    {
        var value = $"EXT{Guid.NewGuid():N}";
        return value[..20];
    }

    private static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    private static string HashToken(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }

    private async Task<string?> ResolveRoleAsync(int userId, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Tourists.ExistsAsync(t => t.UserId == userId && !t.IsDeleted, cancellationToken))
            return ApplicationRoles.Tourist;

        return await _unitOfWork.Employees.GetRoleByUserIdAsync(userId, cancellationToken);
    }
}

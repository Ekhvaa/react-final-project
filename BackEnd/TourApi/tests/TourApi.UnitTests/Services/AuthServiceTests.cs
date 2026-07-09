using Microsoft.Extensions.Options;
using Moq;
using TourApi.Common;
using TourApi.Constants;
using TourApi.DTOs.Auth;
using TourApi.Exceptions;
using TourApi.Factories;
using TourApi.Models;
using TourApi.Repositories;
using TourApi.Services;
using TourApi.UnitTests.TestSupport;
using Xunit;

namespace TourApi.UnitTests.Services;

public sealed class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_ShouldCreateTouristUser_SendConfirmation_AndReturnTouristToken()
    {
        var fixture = new AuthFixture();
        fixture.Users.Setup(x => x.IsUsernameTakenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        fixture.Users.Setup(x => x.GetActiveByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        fixture.Tourists.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Tourist, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        fixture.Employees.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var response = await fixture.Service.RegisterAsync(TestData.RegisterRequest(), "127.0.0.1");

        Assert.Equal(ApplicationRoles.Tourist, response.Role);
        Assert.Equal("jwt-token", response.Token);
        Assert.False(response.EmailConfirmed);
        fixture.Users.Verify(x => x.Add(It.IsAny<User>()), Times.Once);
        fixture.Tourists.Verify(x => x.Add(It.IsAny<Tourist>()), Times.Once);
        fixture.EmailConfirmationTokens.Verify(x => x.Add(It.IsAny<EmailConfirmationToken>()), Times.Once);
        fixture.RefreshTokens.Verify(x => x.Add(It.IsAny<RefreshToken>()), Times.Once);
        fixture.EmailSender.Verify(x => x.SendEmailConfirmationAsync("lado@example.com", It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Fact]
    public async Task RegisterAsync_WhenUsernameTaken_ShouldThrowDuplicateResourceException()
    {
        var fixture = new AuthFixture();
        fixture.Users.Setup(x => x.IsUsernameTakenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<DuplicateResourceException>(() =>
            fixture.Service.RegisterAsync(TestData.RegisterRequest()));
    }

    [Fact]
    public async Task RegisterAsync_WhenGenderInvalid_ShouldThrowInvalidOperationException()
    {
        var fixture = new AuthFixture();
        var request = TestData.RegisterRequest();
        request.Gender = 'X';

        await Assert.ThrowsAsync<InvalidOperationException>(() => fixture.Service.RegisterAsync(request));
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ShouldReturnNull()
    {
        var fixture = new AuthFixture();
        var user = new User { Id = 5, Username = "lado", PasswordHash = new byte[64], EmailConfirmed = true };
        fixture.Users.Setup(x => x.GetActiveByUsernameAsync("lado", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        fixture.PasswordHasher.Setup(x => x.Verify("wrong", user.PasswordHash)).Returns(false);

        var result = await fixture.Service.LoginAsync(new LoginRequest { Username = "lado", Password = "wrong" });

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_WhenEmailConfirmationRequiredAndEmailNotConfirmed_ShouldThrowUnauthorizedAccessException()
    {
        var fixture = new AuthFixture(requireConfirmedEmail: true);
        var user = new User { Id = 5, Username = "lado", PasswordHash = new byte[64], EmailConfirmed = false };
        fixture.Users.Setup(x => x.GetActiveByUsernameAsync("lado", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        fixture.PasswordHasher.Setup(x => x.Verify("Test123!", user.PasswordHash)).Returns(true);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            fixture.Service.LoginAsync(new LoginRequest { Username = "lado", Password = "Test123!" }));
    }

    [Fact]
    public async Task LoginAsync_WithTouristUser_ShouldReturnAuthResponseAndRefreshToken()
    {
        var fixture = new AuthFixture();
        var user = new User { Id = 5, Username = "lado", PasswordHash = new byte[64], EmailConfirmed = true };
        fixture.Users.Setup(x => x.GetActiveByUsernameAsync("lado", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        fixture.PasswordHasher.Setup(x => x.Verify("Test123!", user.PasswordHash)).Returns(true);
        fixture.Tourists.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Tourist, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var response = await fixture.Service.LoginAsync(new LoginRequest { Username = "lado", Password = "Test123!" });

        Assert.NotNull(response);
        Assert.Equal(ApplicationRoles.Tourist, response!.Role);
        Assert.Equal("jwt-token", response.Token);
        Assert.False(string.IsNullOrWhiteSpace(response.RefreshToken));
        fixture.RefreshTokens.Verify(x => x.Add(It.IsAny<RefreshToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginWithGoogleAsync_WhenNewExternalUser_ShouldCreateTouristAndExternalLogin()
    {
        var fixture = new AuthFixture();
        fixture.GoogleExternalAuth.Setup(x => x.GetUserInfoAsync("google-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExternalUserInfo
            {
                Provider = "Google",
                ProviderUserId = "g-1",
                Email = "google@example.com",
                FirstName = "Google",
                LastName = "User"
            });
        fixture.ExternalLogins.Setup(x => x.GetByProviderAsync("Google", "g-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ExternalLogin?)null);
        fixture.Users.Setup(x => x.GetActiveByEmailAsync("google@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        fixture.Users.Setup(x => x.IsUsernameTakenAsync("google", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var response = await fixture.Service.LoginWithGoogleAsync(new GoogleLoginRequest { IdToken = "google-token" });

        Assert.Equal(ApplicationRoles.Tourist, response.Role);
        fixture.Users.Verify(x => x.Add(It.IsAny<User>()), Times.Once);
        fixture.Tourists.Verify(x => x.Add(It.IsAny<Tourist>()), Times.Once);
        fixture.ExternalLogins.Verify(x => x.Add(It.Is<ExternalLogin>(login => login.Provider == "Google" && login.ProviderUserId == "g-1")), Times.Once);
    }

    [Fact]
    public async Task RefreshAsync_WithActiveToken_ShouldRevokeOldTokenAndReturnReplacement()
    {
        var fixture = new AuthFixture();
        var user = new User { Id = 5, Username = "lado", PasswordHash = new byte[64], EmailConfirmed = true };
        var storedToken = new RefreshToken
        {
            UserId = user.Id,
            User = user,
            TokenHash = "old-hash",
            ExpiresAtUtc = DateTime.UtcNow.AddDays(1)
        };
        fixture.RefreshTokens.Setup(x => x.GetActiveByHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(storedToken);
        fixture.Tourists.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Tourist, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var response = await fixture.Service.RefreshAsync(new RefreshTokenRequest { RefreshToken = "old-refresh-token" }, "127.0.0.1");

        Assert.Equal(ApplicationRoles.Tourist, response.Role);
        Assert.NotNull(storedToken.RevokedAtUtc);
        Assert.Equal("127.0.0.1", storedToken.RevokedByIp);
        Assert.False(string.IsNullOrWhiteSpace(storedToken.ReplacedByTokenHash));
        fixture.RefreshTokens.Verify(x => x.Add(It.IsAny<RefreshToken>()), Times.Once);
    }


    [Fact]
    public async Task ForgotPasswordAsync_WhenUserExists_ShouldCreateResetTokenAndSendEmail()
    {
        var fixture = new AuthFixture();
        var user = new User { Id = 5, Username = "lado", PasswordHash = new byte[64], EmailConfirmed = true };
        fixture.Users.Setup(x => x.GetActiveByEmailAsync("lado@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        await fixture.Service.ForgotPasswordAsync(new ForgotPasswordRequest { Email = "lado@example.com" });

        fixture.PasswordResetTokens.Verify(x => x.Add(It.Is<PasswordResetToken>(token => token.UserId == user.Id)), Times.Once);
        fixture.EmailSender.Verify(x => x.SendPasswordResetAsync("lado@example.com", It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResetPasswordAsync_WithActiveToken_ShouldUpdatePasswordAndMarkTokenUsed()
    {
        var fixture = new AuthFixture();
        var user = new User { Id = 5, Username = "lado", PasswordHash = new byte[64], EmailConfirmed = false };
        var resetToken = new PasswordResetToken
        {
            UserId = user.Id,
            User = user,
            TokenHash = "hash",
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(10)
        };
        fixture.PasswordResetTokens.Setup(x => x.GetActiveByHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resetToken);
        var newHash = Enumerable.Repeat((byte)8, 64).ToArray();
        fixture.PasswordHasher.Setup(x => x.Hash("NewPassword123!"))
            .Returns(newHash);

        await fixture.Service.ResetPasswordAsync(new ResetPasswordRequest
        {
            Token = "reset-token",
            NewPassword = "NewPassword123!"
        });

        Assert.Same(newHash, user.PasswordHash);
        Assert.True(user.EmailConfirmed);
        Assert.NotNull(resetToken.UsedAtUtc);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConfirmEmailAsync_WithActiveToken_ShouldConfirmEmailAndMarkTokenUsed()
    {
        var fixture = new AuthFixture();
        var user = new User { Id = 5, Username = "lado", PasswordHash = new byte[64], EmailConfirmed = false };
        var confirmationToken = new EmailConfirmationToken
        {
            UserId = user.Id,
            User = user,
            TokenHash = "hash",
            ExpiresAtUtc = DateTime.UtcNow.AddHours(1)
        };
        fixture.EmailConfirmationTokens.Setup(x => x.GetActiveByHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(confirmationToken);

        await fixture.Service.ConfirmEmailAsync(new ConfirmEmailRequest { Token = "confirm-token" });

        Assert.True(user.EmailConfirmed);
        Assert.NotNull(confirmationToken.UsedAtUtc);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private sealed class AuthFixture
    {
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<IUserRepository> Users { get; } = new();
        public Mock<ITouristRepository> Tourists { get; } = new();
        public Mock<IEmployeeRepository> Employees { get; } = new();
        public Mock<IExternalLoginRepository> ExternalLogins { get; } = new();
        public Mock<IRefreshTokenRepository> RefreshTokens { get; } = new();
        public Mock<IEmailConfirmationTokenRepository> EmailConfirmationTokens { get; } = new();
        public Mock<IPasswordResetTokenRepository> PasswordResetTokens { get; } = new();
        public Mock<IPasswordHasher> PasswordHasher { get; } = new();
        public Mock<IJwtTokenService> JwtTokenService { get; } = new();
        public Mock<IGoogleExternalAuthService> GoogleExternalAuth { get; } = new();
        public Mock<IFacebookExternalAuthService> FacebookExternalAuth { get; } = new();
        public Mock<IEmailSender> EmailSender { get; } = new();
        public AuthService Service { get; }

        public AuthFixture(bool requireConfirmedEmail = false)
        {
            UnitOfWork.SetupGet(x => x.Users).Returns(Users.Object);
            UnitOfWork.SetupGet(x => x.Tourists).Returns(Tourists.Object);
            UnitOfWork.SetupGet(x => x.Employees).Returns(Employees.Object);
            UnitOfWork.SetupGet(x => x.ExternalLogins).Returns(ExternalLogins.Object);
            UnitOfWork.SetupGet(x => x.RefreshTokens).Returns(RefreshTokens.Object);
            UnitOfWork.SetupGet(x => x.EmailConfirmationTokens).Returns(EmailConfirmationTokens.Object);
            UnitOfWork.SetupGet(x => x.PasswordResetTokens).Returns(PasswordResetTokens.Object);
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            PasswordHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns(Enumerable.Repeat((byte)9, 64).ToArray());
            JwtTokenService.Setup(x => x.GenerateToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(("jwt-token", DateTime.UtcNow.AddMinutes(60)));

            Service = new AuthService(
                UnitOfWork.Object,
                PasswordHasher.Object,
                JwtTokenService.Object,
                new UserFactory(),
                GoogleExternalAuth.Object,
                FacebookExternalAuth.Object,
                EmailSender.Object,
                Options.Create(new RefreshTokenOptions { ExpiryDays = 14 }),
                Options.Create(new EmailOptions { RequireConfirmedEmail = requireConfirmedEmail, ConfirmationTokenExpiryHours = 24, PasswordResetTokenExpiryMinutes = 30 }));
        }
    }
}

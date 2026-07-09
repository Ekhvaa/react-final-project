using TourApi.Constants;
using TourApi.DTOs.Auth;
using TourApi.Factories;
using TourApi.Models;
using TourApi.UnitTests.TestSupport;
using Xunit;

namespace TourApi.UnitTests.Factories;

public sealed class UserFactoryTests
{
    private readonly UserFactory _factory = new();

    [Fact]
    public void CreateTouristUser_ShouldCreateUnconfirmedUserAndTourist()
    {
        var request = TestData.RegisterRequest();
        var hash = Enumerable.Repeat((byte)1, 64).ToArray();

        var (user, tourist) = _factory.CreateTouristUser(request, hash);

        Assert.Equal(request.Username, user.Username);
        Assert.Same(hash, user.PasswordHash);
        Assert.False(user.EmailConfirmed);
        Assert.Same(user, tourist.User);
        Assert.Equal(request.FirstName, tourist.FirstName);
        Assert.Equal(request.LastName, tourist.LastName);
        Assert.Equal(request.Email, tourist.Email);
        Assert.Equal(request.NationalId, tourist.NationalId);
    }

    [Fact]
    public void CreateExternalTouristUser_ShouldCreateConfirmedTouristAndExternalLogin()
    {
        var externalUser = new ExternalUserInfo
        {
            Provider = "Google",
            ProviderUserId = "google-user-id",
            Email = "external@example.com",
            FirstName = "External",
            LastName = "Person"
        };
        var hash = Enumerable.Repeat((byte)2, 64).ToArray();

        var (user, tourist, externalLogin) = _factory.CreateExternalTouristUser(
            externalUser,
            "external",
            hash,
            "EXT-NID");

        Assert.True(user.EmailConfirmed);
        Assert.Equal("external", user.Username);
        Assert.Equal(externalUser.Email, tourist.Email);
        Assert.Same(user, tourist.User);
        Assert.Same(user, externalLogin.User);
        Assert.Equal("Google", externalLogin.Provider);
        Assert.Equal("google-user-id", externalLogin.ProviderUserId);
    }

    [Theory]
    [InlineData(ApplicationRoles.TravelAgent, typeof(TravelAgent))]
    [InlineData(ApplicationRoles.TourGuide, typeof(TourGuide))]
    [InlineData(ApplicationRoles.Admin, typeof(Admin))]
    public void CreateEmployeeUser_ShouldCreateRequestedEmployeeType(string role, Type expectedType)
    {
        var request = TestData.EmployeeCreateRequest(role);
        var hash = Enumerable.Repeat((byte)3, 64).ToArray();

        var (user, employee) = _factory.CreateEmployeeUser(request, hash);

        Assert.True(user.EmailConfirmed);
        Assert.IsType(expectedType, employee);
        Assert.Same(user, employee.User);
        Assert.Equal(request.Email, employee.Email);
        Assert.Equal(request.Experience, employee.Experience);
    }

    [Fact]
    public void CreateEmployeeUser_WithUnsupportedRole_ShouldThrow()
    {
        var request = TestData.EmployeeCreateRequest("Hacker");

        Assert.Throws<InvalidOperationException>(() =>
            _factory.CreateEmployeeUser(request, new byte[64]));
    }
}

using TourApi.Constants;
using TourApi.DTOs.Auth;
using TourApi.DTOs.Users;
using TourApi.Models;

namespace TourApi.Factories;

public sealed class UserFactory : IUserFactory
{
    public (User User, Tourist Tourist) CreateTouristUser(RegisterRequest request, byte[] passwordHash)
    {
        var user = new User
        {
            Username = request.Username,
            PasswordHash = passwordHash,
            EmailConfirmed = false
        };

        var tourist = new Tourist
        {
            User = user,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            ContactPhone = request.ContactPhone,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            NationalId = request.NationalId
        };

        return (user, tourist);
    }

    public (User User, Tourist Tourist, ExternalLogin ExternalLogin) CreateExternalTouristUser(
        ExternalUserInfo externalUser,
        string username,
        byte[] passwordHash,
        string nationalId)
    {
        var user = new User
        {
            Username = username,
            PasswordHash = passwordHash,
            EmailConfirmed = true
        };

        var tourist = new Tourist
        {
            User = user,
            FirstName = string.IsNullOrWhiteSpace(externalUser.FirstName) ? "External" : externalUser.FirstName,
            LastName = string.IsNullOrWhiteSpace(externalUser.LastName) ? "User" : externalUser.LastName,
            Email = externalUser.Email,
            ContactPhone = string.Empty,
            DateOfBirth = new DateTime(1900, 1, 1),
            Gender = 'M',
            NationalId = nationalId
        };

        var externalLogin = new ExternalLogin
        {
            User = user,
            Provider = externalUser.Provider,
            ProviderUserId = externalUser.ProviderUserId,
            Email = externalUser.Email
        };

        return (user, tourist, externalLogin);
    }

    public (User User, Employee Employee) CreateEmployeeUser(EmployeeCreateRequest request, byte[] passwordHash)
    {
        var user = new User
        {
            Username = request.Username,
            PasswordHash = passwordHash,
            EmailConfirmed = true
        };

        Employee employee = request.Role switch
        {
            ApplicationRoles.TravelAgent => new TravelAgent(),
            ApplicationRoles.TourGuide => new TourGuide(),
            ApplicationRoles.Admin => new Admin(),
            _ => throw new InvalidOperationException("Unsupported employee role.")
        };

        employee.User = user;
        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;
        employee.Email = request.Email;
        employee.ContactPhone = request.ContactPhone;
        employee.DateOfBirth = request.DateOfBirth;
        employee.Gender = request.Gender;
        employee.NationalId = request.NationalId;
        employee.Experience = request.Experience;

        return (user, employee);
    }
}

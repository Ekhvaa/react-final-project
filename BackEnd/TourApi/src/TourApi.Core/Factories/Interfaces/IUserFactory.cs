using TourApi.DTOs.Auth;
using TourApi.DTOs.Users;
using TourApi.Models;

namespace TourApi.Factories;

public interface IUserFactory
{
    (User User, Tourist Tourist) CreateTouristUser(RegisterRequest request, byte[] passwordHash);
    (User User, Tourist Tourist, ExternalLogin ExternalLogin) CreateExternalTouristUser(
        ExternalUserInfo externalUser,
        string username,
        byte[] passwordHash,
        string nationalId);
    (User User, Employee Employee) CreateEmployeeUser(EmployeeCreateRequest request, byte[] passwordHash);
}

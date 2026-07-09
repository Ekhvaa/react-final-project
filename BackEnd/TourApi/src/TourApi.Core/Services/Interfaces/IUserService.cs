using TourApi.DTOs.Users;

namespace TourApi.Services;

public interface IUserService
{
    Task<TouristDto?> GetTouristByUserIdAsync(int userId);
    Task<bool> UpdateTouristAsync(int userId, TouristUpdateRequest request);
    Task<List<TouringHistoryDto>> GetTouringHistoryAsync(int touristId);
}
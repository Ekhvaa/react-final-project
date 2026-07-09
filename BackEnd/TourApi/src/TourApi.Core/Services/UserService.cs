using AutoMapper;
using TourApi.DTOs.Users;
using TourApi.Repositories;

namespace TourApi.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TouristDto?> GetTouristByUserIdAsync(int userId)
    {
        var tourist = await _unitOfWork.Tourists.GetActiveByUserIdWithUserAsync(userId);

        return tourist is null ? null : _mapper.Map<TouristDto>(tourist);
    }

    public async Task<bool> UpdateTouristAsync(int userId, TouristUpdateRequest request)
    {
        var tourist = await _unitOfWork.Tourists.GetActiveByUserIdWithUserAsync(userId);

        if (tourist is null) return false;

        if (!string.Equals(tourist.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            tourist.User.EmailConfirmed = false;
        }

        _mapper.Map(request, tourist);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<List<TouringHistoryDto>> GetTouringHistoryAsync(int touristId)
    {
        var history = await _unitOfWork.TouringHistories.ListForTouristAsync(touristId);

        return _mapper.Map<List<TouringHistoryDto>>(history);
    }
}

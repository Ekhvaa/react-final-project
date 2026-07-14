using TourApi.DTOs.Common;
using TourApi.DTOs.Tours;

namespace TourApi.Services;

public interface ITourService
{
    Task<PagedResult<TourSummaryDto>> SearchAsync(TourSearchQuery query, CancellationToken cancellationToken = default);
    Task<TourDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TourDto> CreateAsync(TourCreateRequest request, int currentUserId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, TourUpdateRequest request, int currentUserId, string currentUserRole, CancellationToken cancellationToken = default);
    Task<bool> AssignGuideAsync(int id, AssignTourGuideRequest request, int currentUserId, string currentUserRole, CancellationToken cancellationToken = default);
    Task<bool> AssignTravelAgentAsync(int tourId,AssignTravelAgentRequest request,CancellationToken cancellationToken = default);
    Task<List<TourSummaryDto>> GetAssignedToursForGuideAsync(int currentUserId, CancellationToken cancellationToken = default);
}

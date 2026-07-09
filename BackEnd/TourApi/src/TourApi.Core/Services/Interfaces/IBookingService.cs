using TourApi.DTOs.Bookings;

namespace TourApi.Services;

public interface IBookingService
{
    Task<BookingDto> CreateBookingAsync(int touristId, CreateBookingRequest request, CancellationToken cancellationToken = default);
    Task<List<BookingDto>> GetBookingsForTouristAsync(int touristId, CancellationToken cancellationToken = default);
    Task<List<BookingDto>> GetBookingsForTravelAgentAsync(int travelAgentId, CancellationToken cancellationToken = default);
    Task<BookingDto?> GetByIdForCurrentUserAsync(int id, int currentUserId, string currentUserRole, CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusAsync(int id, UpdateBookingStatusRequest request, int currentUserId, string currentUserRole, CancellationToken cancellationToken = default);
    Task<bool> CancelOwnBookingAsync(int id, int currentUserId, CancellationToken cancellationToken = default);
}

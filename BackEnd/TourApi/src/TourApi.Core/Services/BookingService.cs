using AutoMapper;
using TourApi.Constants;
using TourApi.DTOs.Bookings;
using TourApi.Exceptions;
using TourApi.Factories;
using TourApi.Models;
using TourApi.Repositories;

namespace TourApi.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IBookingFactory _bookingFactory;

    public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IBookingFactory bookingFactory)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _bookingFactory = bookingFactory;
    }

    public async Task<BookingDto> CreateBookingAsync(
        int touristId,
        CreateBookingRequest request,
        CancellationToken cancellationToken = default)
    {
        var touristExists = await _unitOfWork.Tourists.ExistsAsync(
            tourist => tourist.Id == touristId && !tourist.IsDeleted,
            cancellationToken);
        if (!touristExists)
            throw new ResourceNotFoundException("Tourist was not found.");

        var tour = await _unitOfWork.Tours.GetActiveWithDetailsByIdAsync(request.TourId, cancellationToken);
        if (tour is null)
            throw new ResourceNotFoundException("Tour was not found.");

        if (!tour.TourDetails.Any())
            throw new BusinessRuleException("Tour has no itinerary and cannot be booked.");

        var firstDeparture = tour.TourDetails.Min(detail => detail.EstimatedArrivalDate);
        if (firstDeparture <= DateTime.UtcNow)
            throw new BusinessRuleException("Cannot book a tour that already started or is in the past.");

        var agentExists = await _unitOfWork.Employees.TravelAgentExistsAsync(request.TravelAgentId, cancellationToken);
        if (!agentExists)
            throw new ResourceNotFoundException("Travel agent was not found.");

        if (await _unitOfWork.Bookings.HasOpenBookingAsync(touristId, request.TourId, cancellationToken))
            throw new DuplicateResourceException("You already have an active booking for this tour.");

        var booking = _bookingFactory.Create(touristId, request, tour.CurrentPrice);
        _unitOfWork.Bookings.Add(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _unitOfWork.Bookings.GetWithDetailsByIdAsync(booking.Id, cancellationToken);
        return _mapper.Map<BookingDto>(created!);
    }

    public async Task<List<BookingDto>> GetBookingsForTouristAsync(int touristId, CancellationToken cancellationToken = default)
    {
        var bookings = await _unitOfWork.Bookings.ListForTouristAsync(touristId, cancellationToken);
        return _mapper.Map<List<BookingDto>>(bookings);
    }

    public async Task<List<BookingDto>> GetBookingsForTravelAgentAsync(int travelAgentId, CancellationToken cancellationToken = default)
    {
        var bookings = await _unitOfWork.Bookings.ListForTravelAgentAsync(travelAgentId, cancellationToken);
        return _mapper.Map<List<BookingDto>>(bookings);
    }

    public async Task<BookingDto?> GetByIdForCurrentUserAsync(
        int id,
        int currentUserId,
        string currentUserRole,
        CancellationToken cancellationToken = default)
    {
        var booking = await _unitOfWork.Bookings.GetWithDetailsByIdAsync(id, cancellationToken);
        if (booking is null)
            return null;

        await EnsureCanReadBookingAsync(booking, currentUserId, currentUserRole, cancellationToken);
        return _mapper.Map<BookingDto>(booking);
    }

    public async Task<bool> UpdateStatusAsync(
        int id,
        UpdateBookingStatusRequest request,
        int currentUserId,
        string currentUserRole,
        CancellationToken cancellationToken = default)
    {
        var booking = await _unitOfWork.Bookings.GetWithDetailsByIdAsync(id, cancellationToken);
        if (booking is null)
            return false;

        await EnsureCanManageBookingAsync(booking, currentUserId, currentUserRole, cancellationToken);
        EnsureValidStatusTransition(booking.Status, request.Status, currentUserRole);

        booking.Status = request.Status;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> CancelOwnBookingAsync(int id, int currentUserId, CancellationToken cancellationToken = default)
    {
        var booking = await _unitOfWork.Bookings.GetWithDetailsByIdAsync(id, cancellationToken);
        if (booking is null)
            return false;

        var tourist = await _unitOfWork.Tourists.GetActiveByUserIdAsync(currentUserId, cancellationToken);
        if (tourist is null || booking.TouristId != tourist.Id)
            throw new ForbiddenAccessException("You can only cancel your own booking.");

        if (booking.Status is BookingStatus.Cancelled)
            return true;

        if (booking.Status is BookingStatus.Completed)
            throw new BusinessRuleException("Completed bookings cannot be cancelled.");

        booking.Status = BookingStatus.Cancelled;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task EnsureCanReadBookingAsync(
        Booking booking,
        int currentUserId,
        string currentUserRole,
        CancellationToken cancellationToken)
    {
        if (currentUserRole == ApplicationRoles.Admin)
            return;

        if (currentUserRole == ApplicationRoles.Tourist)
        {
            var tourist = await _unitOfWork.Tourists.GetActiveByUserIdAsync(currentUserId, cancellationToken);
            if (tourist is not null && booking.TouristId == tourist.Id)
                return;
        }

        if (currentUserRole == ApplicationRoles.TravelAgent)
        {
            var employee = await _unitOfWork.Employees.GetActiveByUserIdAsync(currentUserId, cancellationToken);
            if (employee is not null && booking.TravelAgentId == employee.Id)
                return;
        }

        throw new ForbiddenAccessException("You do not have access to this booking.");
    }

    private async Task EnsureCanManageBookingAsync(
        Booking booking,
        int currentUserId,
        string currentUserRole,
        CancellationToken cancellationToken)
    {
        if (currentUserRole == ApplicationRoles.Admin)
            return;

        if (currentUserRole != ApplicationRoles.TravelAgent)
            throw new ForbiddenAccessException("Only travel agents or admins can update booking status.");

        var employee = await _unitOfWork.Employees.GetActiveByUserIdAsync(currentUserId, cancellationToken);
        if (employee is null || booking.TravelAgentId != employee.Id)
            throw new ForbiddenAccessException("You can only update bookings assigned to you.");
    }

    private static void EnsureValidStatusTransition(BookingStatus currentStatus, BookingStatus requestedStatus, string currentUserRole)
    {
        if (currentStatus == requestedStatus)
            return;

        if (currentUserRole == ApplicationRoles.Admin)
            return;

        var valid = currentStatus switch
        {
            BookingStatus.Pending => requestedStatus is BookingStatus.Confirmed or BookingStatus.Cancelled,
            BookingStatus.Confirmed => requestedStatus is BookingStatus.Completed or BookingStatus.Cancelled,
            BookingStatus.Cancelled => false,
            BookingStatus.Completed => false,
            _ => false
        };

        if (!valid)
            throw new BusinessRuleException($"Cannot change booking status from {currentStatus} to {requestedStatus}.");
    }
}

using AutoMapper;
using TourApi.Constants;
using TourApi.DTOs.Common;
using TourApi.DTOs.Tours;
using TourApi.Exceptions;
using TourApi.Factories;
using TourApi.QueryBuilders;
using TourApi.Repositories;

namespace TourApi.Services;

public class TourService : ITourService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITourFactory _tourFactory;
    private readonly ITourSearchQueryBuilder _searchQueryBuilder;

    public TourService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ITourFactory tourFactory,
        ITourSearchQueryBuilder searchQueryBuilder)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _tourFactory = tourFactory;
        _searchQueryBuilder = searchQueryBuilder;
    }

    public async Task<PagedResult<TourSummaryDto>> SearchAsync(
        TourSearchQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = query.PageSize is > 0 and <= 100 ? query.PageSize : 20;

        var (tours, totalCount) = await _unitOfWork.Tours.SearchAsync(
            query,
            _searchQueryBuilder,
            page,
            pageSize,
            cancellationToken);

        return new PagedResult<TourSummaryDto>
        {
            Items = _mapper.Map<List<TourSummaryDto>>(tours),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<TourDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var tour = await _unitOfWork.Tours.GetActiveWithDetailsByIdAsync(id, cancellationToken);
        return tour is null ? null : _mapper.Map<TourDto>(tour);
    }

    public async Task<TourDto> CreateAsync(
        TourCreateRequest request,
        int currentUserId,
        CancellationToken cancellationToken = default)
    {
        await EnsureTourCodeIsUniqueAsync(request.Code, null, cancellationToken);
        await ValidateItineraryAsync(request.Itinerary, cancellationToken);

        var employee = await _unitOfWork.Employees.GetActiveByUserIdAsync(currentUserId, cancellationToken);
        if (employee is null)
            throw new ForbiddenAccessException("Only employees can create tours.");

        var tour = _tourFactory.Create(request);
        tour.CreatedByEmployeeId = employee.Id;

        _unitOfWork.Tours.Add(tour);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(tour.Id, cancellationToken))!;
    }

    public async Task<bool> UpdateAsync(
        int id,
        TourUpdateRequest request,
        int currentUserId,
        string currentUserRole,
        CancellationToken cancellationToken = default)
    {
        var tour = await _unitOfWork.Tours.GetActiveForUpdateAsync(id, cancellationToken);

        if (tour is null) return false;

        await EnsureCanManageTourAsync(tour.CreatedByEmployeeId, currentUserId, currentUserRole, cancellationToken);
        await EnsureTourCodeIsUniqueAsync(request.Code, id, cancellationToken);
        await ValidateItineraryAsync(request.Itinerary, cancellationToken);

        _tourFactory.Update(tour, request);
        _unitOfWork.TourDetails.RemoveRange(tour.TourDetails);
        _unitOfWork.TourDetails.AddRange(_tourFactory.CreateItinerary(tour.Id, request.Itinerary));

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }


    public async Task<bool> AssignGuideAsync(
        int id,
        AssignTourGuideRequest request,
        int currentUserId,
        string currentUserRole,
        CancellationToken cancellationToken = default)
    {
        var tour = await _unitOfWork.Tours.GetActiveForUpdateAsync(id, cancellationToken);
        if (tour is null)
            return false;

        await EnsureCanManageTourAsync(tour.CreatedByEmployeeId, currentUserId, currentUserRole, cancellationToken);

        var guideExists = await _unitOfWork.Employees.TourGuideExistsAsync(request.TourGuideId, cancellationToken);
        if (!guideExists)
            throw new ResourceNotFoundException("Tour guide was not found.");

        tour.AssignedTourGuideId = request.TourGuideId;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<List<TourSummaryDto>> GetAssignedToursForGuideAsync(
        int currentUserId,
        CancellationToken cancellationToken = default)
    {
        var employee = await _unitOfWork.Employees.GetActiveByUserIdAsync(currentUserId, cancellationToken);
        if (employee is null || employee.GetType().Name != ApplicationRoles.TourGuide)
            throw new ForbiddenAccessException("Only tour guides can view assigned tours.");

        var tours = await _unitOfWork.Tours.ListAssignedToTourGuideAsync(employee.Id, cancellationToken);
        return _mapper.Map<List<TourSummaryDto>>(tours);
    }

    private async Task EnsureCanManageTourAsync(
        int? createdByEmployeeId,
        int currentUserId,
        string currentUserRole,
        CancellationToken cancellationToken)
    {
        if (currentUserRole == ApplicationRoles.Admin)
            return;

        if (currentUserRole != ApplicationRoles.TravelAgent)
            throw new ForbiddenAccessException("Only travel agents or admins can manage tours.");

        if (!createdByEmployeeId.HasValue)
            return;

        var employee = await _unitOfWork.Employees.GetActiveByUserIdAsync(currentUserId, cancellationToken);
        if (employee is null || employee.Id != createdByEmployeeId.Value)
            throw new ForbiddenAccessException("You can only manage tours created by you.");
    }

    private async Task EnsureTourCodeIsUniqueAsync(string code, int? currentTourId, CancellationToken cancellationToken)
    {
        var codeTaken = await _unitOfWork.Tours.IsCodeTakenAsync(code, currentTourId, cancellationToken);

        if (codeTaken)
            throw new DuplicateResourceException("Tour code is already in use.");
    }

    private async Task ValidateItineraryAsync(
        IReadOnlyCollection<TourItineraryLegCreateRequest> itinerary,
        CancellationToken cancellationToken)
    {
        if (itinerary.Count == 0)
            throw new InvalidOperationException("Tour itinerary must contain at least one leg.");

        if (itinerary.Select(leg => leg.Sequence).Distinct().Count() != itinerary.Count)
            throw new InvalidOperationException("Tour itinerary sequence numbers must be unique.");

        foreach (var leg in itinerary)
        {
            if (leg.EstimatedDepartureDate <= leg.EstimatedArrivalDate)
                throw new InvalidOperationException("Each itinerary departure date must be after arrival date.");
        }

        var ordered = itinerary.OrderBy(leg => leg.Sequence).ToList();
        for (var i = 1; i < ordered.Count; i++)
        {
            if (ordered[i].EstimatedArrivalDate < ordered[i - 1].EstimatedDepartureDate)
                throw new InvalidOperationException("Itinerary dates cannot overlap backwards.");
        }

        var cityIds = itinerary.Select(leg => leg.CityId).Distinct().ToList();
        var existingCityCount = await _unitOfWork.Cities.CountActiveByIdsAsync(cityIds, cancellationToken);

        if (existingCityCount != cityIds.Count)
            throw new InvalidOperationException("One or more itinerary cities were not found.");

        var hotelIds = itinerary
            .Where(leg => leg.HotelId.HasValue)
            .Select(leg => leg.HotelId!.Value)
            .Distinct()
            .ToList();

        if (hotelIds.Count == 0) return;

        var existingHotelCount = await _unitOfWork.Hotels.CountActiveByIdsAsync(hotelIds, cancellationToken);

        if (existingHotelCount != hotelIds.Count)
            throw new InvalidOperationException("One or more itinerary hotels were not found.");
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourApi.Constants;
using TourApi.DTOs.Bookings;
using TourApi.Services;

namespace TourApi.Controllers;

[Route("api/bookings")]
[Authorize]
public class BookingsController : ApiControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IUserService _userService;
    private readonly IEmployeeService _employeeService;

    public BookingsController(
        IBookingService bookingService,
        IUserService userService,
        IEmployeeService employeeService)
    {
        _bookingService = bookingService;
        _userService = userService;
        _employeeService = employeeService;
    }

    [HttpPost]
    [Authorize(Roles = ApplicationRoles.Tourist)]
    public async Task<ActionResult<BookingDto>> Create(
        [FromBody] CreateBookingRequest request,
        CancellationToken cancellationToken)
    {
        var tourist = await _userService.GetTouristByUserIdAsync(CurrentUserId);
        if (tourist is null) return Forbid();

        var booking = await _bookingService.CreateBookingAsync(tourist.Id, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
    }

    [HttpGet("mine")]
    [Authorize(Roles = ApplicationRoles.Tourist)]
    public async Task<ActionResult<List<BookingDto>>> GetMine(CancellationToken cancellationToken)
    {
        var tourist = await _userService.GetTouristByUserIdAsync(CurrentUserId);
        if (tourist is null) return Forbid();

        return Ok(await _bookingService.GetBookingsForTouristAsync(tourist.Id, cancellationToken));
    }

    [HttpGet("agent/mine")]
    [Authorize(Roles = ApplicationRoles.TravelAgent)]
    public async Task<ActionResult<List<BookingDto>>> GetMineForAgent(CancellationToken cancellationToken)
    {
        var employee = await _employeeService.GetByUserIdAsync(CurrentUserId, cancellationToken);
        if (employee is null) return Forbid();

        return Ok(await _bookingService.GetBookingsForTravelAgentAsync(employee.Id, cancellationToken));
    }

    [HttpGet("agent/{travelAgentId:int}")]
    [Authorize(Roles = ApplicationRoles.Admin)]
    public async Task<ActionResult<List<BookingDto>>> GetForAgent(
        int travelAgentId,
        CancellationToken cancellationToken)
    {
        return Ok(await _bookingService.GetBookingsForTravelAgentAsync(travelAgentId, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var booking = await _bookingService.GetByIdForCurrentUserAsync(
            id,
            CurrentUserId,
            CurrentUserRole,
            cancellationToken);

        return booking is null ? NotFound() : Ok(booking);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = ApplicationRoles.TravelAgentOrAdmin)]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] UpdateBookingStatusRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _bookingService.UpdateStatusAsync(
            id,
            request,
            CurrentUserId,
            CurrentUserRole,
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPut("{id:int}/cancel")]
    [Authorize(Roles = ApplicationRoles.Tourist)]
    public async Task<IActionResult> CancelOwnBooking(int id, CancellationToken cancellationToken)
    {
        var updated = await _bookingService.CancelOwnBookingAsync(id, CurrentUserId, cancellationToken);
        return updated ? NoContent() : NotFound();
    }
}

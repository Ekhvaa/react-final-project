using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourApi.Constants;
using TourApi.DTOs.Tours;
using TourApi.Services;

namespace TourApi.Controllers;

[Route("api/tour-guides")]
[Authorize(Roles = ApplicationRoles.TourGuide)]
public sealed class TourGuidesController : ApiControllerBase
{
    private readonly ITourService _tourService;

    public TourGuidesController(ITourService tourService)
    {
        _tourService = tourService;
    }

    [HttpGet("assigned-tours")]
    public async Task<ActionResult<List<TourSummaryDto>>> GetAssignedTours(CancellationToken cancellationToken)
    {
        return Ok(await _tourService.GetAssignedToursForGuideAsync(CurrentUserId, cancellationToken));
    }
}

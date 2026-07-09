using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourApi.Constants;
using TourApi.DTOs.Common;
using TourApi.DTOs.Images;
using TourApi.DTOs.Tours;
using TourApi.Services;

namespace TourApi.Controllers;

[Route("api/tours")]
public class ToursController : ApiControllerBase
{
    private readonly ITourService _tourService;
    private readonly IImageService _imageService;

    public ToursController(ITourService tourService, IImageService imageService)
    {
        _tourService = tourService;
        _imageService = imageService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<TourSummaryDto>>> Search(
        [FromQuery] TourSearchQuery query,
        CancellationToken cancellationToken) =>
        Ok(await _tourService.SearchAsync(query, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TourDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var tour = await _tourService.GetByIdAsync(id, cancellationToken);
        return tour is null ? NotFound() : Ok(tour);
    }

    [HttpPost]
    [Authorize(Roles = ApplicationRoles.TravelAgentOrAdmin)]
    public async Task<ActionResult<TourDto>> Create(
        [FromBody] TourCreateRequest request,
        CancellationToken cancellationToken)
    {
        var tour = await _tourService.CreateAsync(request, CurrentUserId, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = tour.Id }, tour);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = ApplicationRoles.TravelAgentOrAdmin)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] TourUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _tourService.UpdateAsync(id, request, CurrentUserId, CurrentUserRole, cancellationToken);
        return updated ? NoContent() : NotFound();
    }


    [HttpPut("{id:int}/guide")]
    [Authorize(Roles = ApplicationRoles.TravelAgentOrAdmin)]
    public async Task<IActionResult> AssignGuide(
        int id,
        [FromBody] AssignTourGuideRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _tourService.AssignGuideAsync(id, request, CurrentUserId, CurrentUserRole, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpGet("{id:int}/images")]
    public async Task<ActionResult<List<ImageDto>>> GetImages(int id, CancellationToken cancellationToken)
    {
        return Ok(await _imageService.GetTourImagesAsync(id, cancellationToken));
    }

    [HttpPost("{id:int}/images")]
    [Authorize(Roles = ApplicationRoles.TravelAgentOrAdmin)]
    [RequestSizeLimit(10_000_000)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ImageDto>> UploadImage(
        int id,
        IFormFile file,
        [FromQuery] bool isCover,
        CancellationToken cancellationToken)
    {
        var upload = new FileUpload
        {
            Stream = file.OpenReadStream(),
            FileName = file.FileName,
            ContentType = file.ContentType,
            Length = file.Length
        };

        var image = await _imageService.AddTourImageAsync(id, upload, isCover, cancellationToken);
        return CreatedAtAction(nameof(GetImages), new { id }, image);
    }
}

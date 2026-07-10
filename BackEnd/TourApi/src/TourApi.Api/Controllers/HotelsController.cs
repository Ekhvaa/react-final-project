using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourApi.Constants;
using TourApi.DTOs.Hotels;
using TourApi.DTOs.Images;
using TourApi.Services;

namespace TourApi.Controllers;

[Route("api/hotels")]
public class HotelsController : ApiControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly IImageService _imageService;

    public HotelsController(IHotelService hotelService, IImageService imageService)
    {
        _hotelService = hotelService;
        _imageService = imageService;
    }

    [HttpGet]
    public async Task<ActionResult<List<HotelDto>>> GetAll([FromQuery] int? cityId) =>
        Ok(await _hotelService.GetHotelsAsync(cityId));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HotelDto>> GetById(int id)
    {
        var hotel = await _hotelService.GetHotelByIdAsync(id);
        return hotel is null ? NotFound() : Ok(hotel);
    }

    [HttpGet("services")]
    public async Task<ActionResult<List<HotelServiceDto>>> GetServices() =>
        Ok(await _hotelService.GetHotelServicesAsync());

    [HttpPost("services")]
    [Authorize(Roles = ApplicationRoles.TravelAgentOrAdmin)]
    public async Task<ActionResult<HotelServiceDto>> CreateService([FromBody] HotelServiceCreateRequest request)
    {
        var service = await _hotelService.CreateHotelServiceAsync(request);
        return CreatedAtAction(nameof(GetServices), routeValues: null, value: service);
    }

    [HttpPost]
    [Authorize(Roles = ApplicationRoles.TravelAgentOrAdmin)]
    public async Task<ActionResult<HotelDto>> Create([FromBody] HotelCreateRequest request)
    {
        var hotel = await _hotelService.CreateHotelAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, hotel);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = ApplicationRoles.TravelAgentOrAdmin)]
    public async Task<IActionResult> Update(int id, [FromBody] HotelUpdateRequest request)
    {
        var updated = await _hotelService.UpdateHotelAsync(id, request);
        return updated ? NoContent() : NotFound();
    }

    [HttpGet("{id:int}/images")]
    public async Task<ActionResult<List<ImageDto>>> GetImages(int id, CancellationToken cancellationToken)
    {
        return Ok(await _imageService.GetHotelImagesAsync(id, cancellationToken));
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

        var image = await _imageService.AddHotelImageAsync(id, upload, isCover, cancellationToken);
        return CreatedAtAction(nameof(GetImages), new { id }, image);
    }
}

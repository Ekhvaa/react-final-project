using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourApi.Constants;
using TourApi.DTOs.Geography;
using TourApi.DTOs.Images;
using TourApi.Services;

namespace TourApi.Controllers;

[Route("api/countries")]
public class CountriesController : ApiControllerBase
{
    private readonly IGeographyService _geographyService;

    public CountriesController(IGeographyService geographyService)
    {
        _geographyService = geographyService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CountryDto>>> GetAll() =>
        Ok(await _geographyService.GetCountriesAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CountryDto>> GetById(int id)
    {
        var country = await _geographyService.GetCountryByIdAsync(id);
        return country is null ? NotFound() : Ok(country);
    }

    [HttpPost]
    [Authorize(Roles = ApplicationRoles.TravelAgentOrAdmin)]
    public async Task<ActionResult<CountryDto>> Create([FromBody] CountryCreateRequest request)
    {
        var country = await _geographyService.CreateCountryAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = country.Id }, country);
    }

    [HttpPost("{id:int}/flag")]
    [Authorize(Roles = ApplicationRoles.TravelAgentOrAdmin)]
    [RequestSizeLimit(10_000_000)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<CountryDto>> UploadFlag(
        int id,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest("Flag image file is required.");

        var upload = new FileUpload
        {
            Stream = file.OpenReadStream(),
            FileName = file.FileName,
            ContentType = file.ContentType,
            Length = file.Length
        };

        var country = await _geographyService.UploadCountryFlagAsync(id, upload, cancellationToken);
        return Ok(country);
    }
}

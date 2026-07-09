using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourApi.Constants;
using TourApi.DTOs.Geography;
using TourApi.Services;

namespace TourApi.Controllers;

[Route("api/cities")]
public class CitiesController : ApiControllerBase
{
    private readonly IGeographyService _geographyService;

    public CitiesController(IGeographyService geographyService)
    {
        _geographyService = geographyService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CityDto>>> GetAll([FromQuery] int? countryId) =>
        Ok(await _geographyService.GetCitiesAsync(countryId));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CityDto>> GetById(int id)
    {
        var city = await _geographyService.GetCityByIdAsync(id);
        return city is null ? NotFound() : Ok(city);
    }

    [HttpPost]
    [Authorize(Roles = ApplicationRoles.TravelAgentOrAdmin)]
    public async Task<ActionResult<CityDto>> Create([FromBody] CityCreateRequest request)
    {
        var city = await _geographyService.CreateCityAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = city.Id }, city);
    }
}

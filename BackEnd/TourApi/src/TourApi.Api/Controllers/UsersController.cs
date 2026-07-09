using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourApi.Constants;
using TourApi.DTOs.Users;
using TourApi.Services;

namespace TourApi.Controllers;

[Route("api/users/me")]
[Authorize(Roles = ApplicationRoles.Tourist)]
public class UsersController : ApiControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<TouristDto>> GetProfile()
    {
        var profile = await _userService.GetTouristByUserIdAsync(CurrentUserId);
        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] TouristUpdateRequest request)
    {
        var updated = await _userService.UpdateTouristAsync(CurrentUserId, request);
        return updated ? NoContent() : NotFound();
    }

    [HttpGet("history")]
    public async Task<ActionResult<List<TouringHistoryDto>>> GetHistory()
    {
        var profile = await _userService.GetTouristByUserIdAsync(CurrentUserId);
        return profile is null
            ? NotFound()
            : Ok(await _userService.GetTouringHistoryAsync(profile.Id));
    }
}

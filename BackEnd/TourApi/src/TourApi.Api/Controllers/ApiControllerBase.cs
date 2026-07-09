using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace TourApi.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    protected string CurrentUserRole =>
        User.FindFirstValue(ClaimTypes.Role)
        ?? User.FindFirstValue("role")
        ?? string.Empty;

    protected string? CurrentIpAddress => HttpContext.Connection.RemoteIpAddress?.ToString();
}

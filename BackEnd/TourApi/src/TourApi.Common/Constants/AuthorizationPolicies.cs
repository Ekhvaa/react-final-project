namespace TourApi.Constants;

public static class AuthorizationPolicies
{
    public const string AdminOnly = nameof(AdminOnly);
    public const string ManageEmployees = nameof(ManageEmployees);
    public const string ManageTours = nameof(ManageTours);
    public const string ManageHotels = nameof(ManageHotels);
    public const string ManageBookings = nameof(ManageBookings);
}

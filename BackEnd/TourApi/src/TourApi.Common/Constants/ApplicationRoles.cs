namespace TourApi.Constants;

public static class ApplicationRoles
{
    public const string Tourist = nameof(Tourist);
    public const string TravelAgent = nameof(TravelAgent);
    public const string TourGuide = nameof(TourGuide);
    public const string Admin = nameof(Admin);

    public const string TravelAgentOrAdmin = TravelAgent + "," + Admin;
}

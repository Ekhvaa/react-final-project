using TourApi.DTOs.Tours;
using TourApi.Factories;
using TourApi.UnitTests.TestSupport;
using Xunit;

namespace TourApi.UnitTests.Factories;

public sealed class TourFactoryTests
{
    private readonly TourFactory _factory = new();

    [Fact]
    public void Create_ShouldCreateTourWithOrderedItinerary()
    {
        var request = TestData.TourCreateRequest();

        var tour = _factory.Create(request);

        Assert.Equal(request.Code, tour.Code);
        Assert.Equal(request.Name, tour.Name);
        Assert.Equal(request.CurrentPrice, tour.CurrentPrice);
        Assert.Equal(2, tour.TourDetails.Count);
        Assert.Equal(new byte[] { 1, 2 }, tour.TourDetails.Select(x => x.Sequence).ToArray());
    }

    [Fact]
    public void Update_ShouldChangeEditableFieldsAndSetUpdateDate()
    {
        var tour = _factory.Create(TestData.TourCreateRequest());
        var request = new TourUpdateRequest
        {
            Code = "GEO-002",
            Name = "Updated Tour",
            CurrentPrice = 999,
            Itinerary = TestData.TourCreateRequest().Itinerary
        };

        _factory.Update(tour, request);

        Assert.Equal("GEO-002", tour.Code);
        Assert.Equal("Updated Tour", tour.Name);
        Assert.Equal(999, tour.CurrentPrice);
        Assert.NotNull(tour.UpdateDate);
    }

    [Fact]
    public void CreateItinerary_ShouldAssignTourIdAndSortBySequence()
    {
        var request = TestData.TourCreateRequest();

        var details = _factory.CreateItinerary(55, request.Itinerary).ToList();

        Assert.All(details, detail => Assert.Equal(55, detail.TourId));
        Assert.Equal(new byte[] { 1, 2 }, details.Select(x => x.Sequence).ToArray());
    }
}

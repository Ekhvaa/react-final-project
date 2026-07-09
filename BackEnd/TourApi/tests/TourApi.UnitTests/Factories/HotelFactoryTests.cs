using TourApi.DTOs.Hotels;
using TourApi.Factories;
using TourApi.Models;
using TourApi.UnitTests.TestSupport;
using Xunit;

namespace TourApi.UnitTests.Factories;

public sealed class HotelFactoryTests
{
    private readonly HotelFactory _factory = new();

    [Fact]
    public void Create_ShouldCreateHotelAndDistinctServiceMappings()
    {
        var request = TestData.HotelCreateRequest();

        var hotel = _factory.Create(request);

        Assert.Equal(request.Name, hotel.Name);
        Assert.Equal(request.StarRating, hotel.StarRating);
        Assert.Equal(request.CityId, hotel.CityId);
        Assert.Equal(new[] { 1, 2 }, hotel.HotelServiceMappings.Select(x => x.HotelServiceId).OrderBy(x => x).ToArray());
    }

    [Fact]
    public void Update_ShouldChangeEditableFieldsAndSetUpdateDate()
    {
        var hotel = new Hotel { Name = "Old", CityId = 1, StarRating = 3 };
        var request = new HotelUpdateRequest
        {
            Name = "New",
            CityId = 2,
            StarRating = 5,
            HotelServiceIds = new List<int> { 1, 2 }
        };

        _factory.Update(hotel, request);

        Assert.Equal("New", hotel.Name);
        Assert.Equal(2, hotel.CityId);
        Assert.Equal((byte)5, hotel.StarRating);
        Assert.NotNull(hotel.UpdateDate);
    }

    [Fact]
    public void CreateServiceMappings_ShouldRemoveDuplicateServiceIds()
    {
        var mappings = _factory.CreateServiceMappings(99, new[] { 1, 1, 2 }).ToList();

        Assert.Equal(2, mappings.Count);
        Assert.All(mappings, mapping => Assert.Equal(99, mapping.HotelId));
        Assert.Equal(new[] { 1, 2 }, mappings.Select(x => x.HotelServiceId).OrderBy(x => x).ToArray());
    }
}

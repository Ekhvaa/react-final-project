using TourApi.DTOs.Tours;
using TourApi.Models;
using TourApi.QueryBuilders;
using Xunit;

namespace TourApi.UnitTests.QueryBuilders;

public sealed class TourSearchQueryBuilderTests
{
    private readonly TourSearchQueryBuilder _builder = new();

    [Fact]
    public void Apply_ShouldExcludeDeletedTours()
    {
        var tours = new[]
        {
            new Tour { Id = 1, Code = "A", Name = "Active", CurrentPrice = 100, IsDeleted = false },
            new Tour { Id = 2, Code = "D", Name = "Deleted", CurrentPrice = 100, IsDeleted = true }
        }.AsQueryable();

        var result = _builder.Apply(tours, new TourSearchQuery()).ToList();

        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
    }

    [Fact]
    public void Apply_ShouldFilterByKeywordAndPriceRange()
    {
        var tours = new[]
        {
            new Tour { Id = 1, Code = "GEO-001", Name = "Georgia Mountains", CurrentPrice = 800 },
            new Tour { Id = 2, Code = "ITA-001", Name = "Italy Coast", CurrentPrice = 1200 },
            new Tour { Id = 3, Code = "GEO-LOW", Name = "Georgia Budget", CurrentPrice = 200 }
        }.AsQueryable();

        var result = _builder.Apply(tours, new TourSearchQuery
        {
            Keyword = "Georgia",
            MinPrice = 300,
            MaxPrice = 1000
        }).ToList();

        Assert.Single(result);
        Assert.Equal("GEO-001", result[0].Code);
    }

    [Fact]
    public void Apply_ShouldFilterByCityCountryAndDepartureDate()
    {
        var now = DateTime.UtcNow;
        var georgia = new Country { Id = 1, Name = "Georgia", IsoName = "GE" };
        var italy = new Country { Id = 2, Name = "Italy", IsoName = "IT" };
        var tours = new[]
        {
            new Tour
            {
                Id = 1,
                Code = "GEO-001",
                Name = "Georgia",
                CurrentPrice = 800,
                TourDetails = new List<TourDetail>
                {
                    new() { CityId = 10, City = new City { Id = 10, CountryId = georgia.Id, Country = georgia, Name = "Tbilisi" }, EstimatedDepartureDate = now.AddDays(10) }
                }
            },
            new Tour
            {
                Id = 2,
                Code = "ITA-001",
                Name = "Italy",
                CurrentPrice = 900,
                TourDetails = new List<TourDetail>
                {
                    new() { CityId = 20, City = new City { Id = 20, CountryId = italy.Id, Country = italy, Name = "Rome" }, EstimatedDepartureDate = now.AddDays(30) }
                }
            }
        }.AsQueryable();

        var result = _builder.Apply(tours, new TourSearchQuery
        {
            CityId = 10,
            CountryId = 1,
            DepartingAfter = now.AddDays(5),
            DepartingBefore = now.AddDays(20)
        }).ToList();

        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
    }
}

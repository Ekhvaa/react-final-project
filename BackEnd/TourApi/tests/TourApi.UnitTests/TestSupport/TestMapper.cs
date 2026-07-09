using AutoMapper;
using TourApi.Mapping;

namespace TourApi.UnitTests.TestSupport;

public static class TestMapper
{
    public static IMapper Create()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<TourApiMappingProfile>());
        config.AssertConfigurationIsValid();
        return config.CreateMapper();
    }
}

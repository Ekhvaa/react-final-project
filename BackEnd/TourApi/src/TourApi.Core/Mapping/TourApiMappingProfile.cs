using AutoMapper;
using TourApi.DTOs.Auth;
using TourApi.DTOs.Bookings;
using TourApi.DTOs.Geography;
using TourApi.DTOs.Hotels;
using TourApi.DTOs.Images;
using TourApi.DTOs.Tours;
using TourApi.DTOs.Users;
using TourApi.Models;

namespace TourApi.Mapping;

public sealed class TourApiMappingProfile : Profile
{
    public TourApiMappingProfile()
    {
        CreateMap<Country, CountryDto>();
        CreateMap<CountryCreateRequest, Country>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
            .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.FlagUrl, opt => opt.Ignore())
            .ForMember(dest => dest.Cities, opt => opt.Ignore());

        CreateMap<City, CityDto>()
            .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.Name));
        CreateMap<CityCreateRequest, City>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Country, opt => opt.Ignore())
            .ForMember(dest => dest.Hotels, opt => opt.Ignore())
            .ForMember(dest => dest.TourDetails, opt => opt.Ignore())
            .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
            .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        CreateMap<HotelService, HotelServiceDto>();
        CreateMap<TourImage, ImageDto>();
        CreateMap<HotelImage, ImageDto>();
        CreateMap<Hotel, HotelDto>()
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
            .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.City.Country.Name))
            .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.HotelServiceMappings.Select(m => m.HotelService)));

        CreateMap<TourDetail, TourItineraryLegDto>()
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel == null ? null : src.Hotel.Name));
        CreateMap<Tour, TourDto>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Itinerary, opt => opt.MapFrom(src => src.TourDetails.OrderBy(td => td.Sequence)))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Where(image => !image.IsDeleted)))
            .ForMember(dest => dest.AssignedTourGuideFullName, opt => opt.MapFrom(src => src.AssignedTourGuide == null ? null : src.AssignedTourGuide.FirstName + " " + src.AssignedTourGuide.LastName));
        CreateMap<Tour, TourSummaryDto>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.StartingCity, opt => opt.MapFrom(src => src.TourDetails.OrderBy(td => td.Sequence).Select(td => td.City.Name).FirstOrDefault()))
            .ForMember(dest => dest.StartingCountry, opt => opt.MapFrom(src => src.TourDetails.OrderBy(td => td.Sequence).Select(td => td.City.Country.Name).FirstOrDefault()))
            .ForMember(dest => dest.EarliestDeparture, opt => opt.MapFrom(src => src.TourDetails.OrderBy(td => td.Sequence).Select(td => (DateTime?)td.EstimatedArrivalDate).FirstOrDefault()))
            .ForMember(dest => dest.DurationDays, opt => opt.MapFrom(src => CalculateDurationDays(src)))
            .ForMember(dest => dest.AssignedTourGuideFullName, opt => opt.MapFrom(src => src.AssignedTourGuide == null ? null : src.AssignedTourGuide.FirstName + " " + src.AssignedTourGuide.LastName));

        CreateMap<Tourist, TouristDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => src.User.EmailConfirmed));
        CreateMap<TouristUpdateRequest, Tourist>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.DateOfBirth, opt => opt.Ignore())
            .ForMember(dest => dest.Gender, opt => opt.Ignore())
            .ForMember(dest => dest.NationalId, opt => opt.Ignore())
            .ForMember(dest => dest.Bookings, opt => opt.Ignore())
            .ForMember(dest => dest.TouringHistories, opt => opt.Ignore())
            .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(_ => DateTime.UtcNow));
        CreateMap<TouringHistory, TouringHistoryDto>()
            .ForMember(dest => dest.TourCode, opt => opt.MapFrom(src => src.Tour.Code))
            .ForMember(dest => dest.TourName, opt => opt.MapFrom(src => src.Tour.Name));

        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.GetType().Name));

        CreateMap<Booking, BookingDto>()
            .ForMember(dest => dest.TourCode, opt => opt.MapFrom(src => src.Tour.Code))
            .ForMember(dest => dest.TourName, opt => opt.MapFrom(src => src.Tour.Name))
            .ForMember(dest => dest.TouristFullName, opt => opt.MapFrom(src => $"{src.Tourist.FirstName} {src.Tourist.LastName}"))
            .ForMember(dest => dest.TravelAgentFullName, opt => opt.MapFrom(src => $"{src.TravelAgent.FirstName} {src.TravelAgent.LastName}"));

        CreateMap<User, CurrentUserDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Role, opt => opt.Ignore());
    }

    private static int CalculateDurationDays(Tour tour)
    {
        var firstLeg = tour.TourDetails.OrderBy(td => td.Sequence).FirstOrDefault();
        var lastLeg = tour.TourDetails.OrderByDescending(td => td.EstimatedDepartureDate).FirstOrDefault();

        return firstLeg is not null && lastLeg is not null
            ? (lastLeg.EstimatedDepartureDate - firstLeg.EstimatedArrivalDate).Days
            : 0;
    }
}

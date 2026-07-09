using AutoMapper;
using TourApi.DTOs.Images;
using TourApi.Exceptions;
using TourApi.Models;
using TourApi.Repositories;

namespace TourApi.Services;

public sealed class ImageService : IImageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;

    public ImageService(IUnitOfWork unitOfWork, IFileStorageService fileStorageService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
    }

    public async Task<ImageDto> AddTourImageAsync(
        int tourId,
        FileUpload file,
        bool isCover,
        CancellationToken cancellationToken = default)
    {
        var tour = await _unitOfWork.Tours.GetActiveByIdAsync(tourId, cancellationToken);
        if (tour is null)
            throw new ResourceNotFoundException("Tour was not found.");

        var stored = await _fileStorageService.SaveImageAsync(file, "tours", cancellationToken);
        var image = new TourImage
        {
            TourId = tourId,
            Url = stored.Url,
            FileName = stored.FileName,
            ContentType = stored.ContentType,
            Length = stored.Length,
            IsCover = isCover
        };

        if (isCover)
        {
            var existingImages = await _unitOfWork.TourImages.ListForTourAsync(tourId, cancellationToken);
            foreach (var existingImage in existingImages)
            {
                existingImage.IsCover = false;
            }
        }

        _unitOfWork.TourImages.Add(image);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ImageDto>(image);
    }

    public async Task<ImageDto> AddHotelImageAsync(
        int hotelId,
        FileUpload file,
        bool isCover,
        CancellationToken cancellationToken = default)
    {
        var hotel = await _unitOfWork.Hotels.GetActiveWithDetailsByIdAsync(hotelId, cancellationToken);
        if (hotel is null)
            throw new ResourceNotFoundException("Hotel was not found.");

        var stored = await _fileStorageService.SaveImageAsync(file, "hotels", cancellationToken);
        var image = new HotelImage
        {
            HotelId = hotelId,
            Url = stored.Url,
            FileName = stored.FileName,
            ContentType = stored.ContentType,
            Length = stored.Length,
            IsCover = isCover
        };

        if (isCover)
        {
            var existingImages = await _unitOfWork.HotelImages.ListForHotelAsync(hotelId, cancellationToken);
            foreach (var existingImage in existingImages)
            {
                existingImage.IsCover = false;
            }
        }

        _unitOfWork.HotelImages.Add(image);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ImageDto>(image);
    }

    public async Task<List<ImageDto>> GetTourImagesAsync(int tourId, CancellationToken cancellationToken = default)
    {
        var images = await _unitOfWork.TourImages.ListForTourAsync(tourId, cancellationToken);
        return _mapper.Map<List<ImageDto>>(images);
    }

    public async Task<List<ImageDto>> GetHotelImagesAsync(int hotelId, CancellationToken cancellationToken = default)
    {
        var images = await _unitOfWork.HotelImages.ListForHotelAsync(hotelId, cancellationToken);
        return _mapper.Map<List<ImageDto>>(images);
    }
}

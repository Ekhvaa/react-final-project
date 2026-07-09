using Microsoft.Extensions.Options;
using TourApi.Common;
using TourApi.DTOs.Images;
using TourApi.Exceptions;
using TourApi.Services;

namespace TourApi.Infrastructure.Security;

public sealed class LocalFileStorageService : IFileStorageService
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    private readonly FileStorageOptions _options;

    public LocalFileStorageService(IOptions<FileStorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<StoredFile> SaveImageAsync(FileUpload file, string folderName, CancellationToken cancellationToken = default)
    {
        if (file.Length <= 0)
            throw new BusinessRuleException("Uploaded image is empty.");

        if (file.Length > _options.MaxImageBytes)
            throw new BusinessRuleException($"Image cannot be larger than {_options.MaxImageBytes} bytes.");

        if (!AllowedContentTypes.Contains(file.ContentType))
            throw new BusinessRuleException("Only jpeg, png, webp, and gif images are allowed.");

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension))
            extension = file.ContentType switch
            {
                "image/png" => ".png",
                "image/webp" => ".webp",
                "image/gif" => ".gif",
                _ => ".jpg"
            };

        var safeFolder = string.Concat(folderName.Where(char.IsLetterOrDigit));
        var newFileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var root = Path.GetFullPath(_options.RootPath);
        var folderPath = Path.Combine(root, safeFolder);
        Directory.CreateDirectory(folderPath);

        var path = Path.Combine(folderPath, newFileName);
        await using (var output = File.Create(path))
        {
            await file.Stream.CopyToAsync(output, cancellationToken);
        }

        var publicPath = $"{_options.PublicBasePath.TrimEnd('/')}/{safeFolder}/{newFileName}";
        return new StoredFile
        {
            Url = publicPath,
            FileName = newFileName,
            ContentType = file.ContentType,
            Length = file.Length
        };
    }
}

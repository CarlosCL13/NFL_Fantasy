using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;

namespace NFLFantasy.Api.DataAccessLayer.StorageManagement;

/// <summary>
/// Handles image storage operations including saving and thumbnail generation
/// </summary>
public interface IImageStorageHandler
{
    Task<(string imageFileName, string thumbnailFileName)> ProcessImageAsync(byte[] imageBytes, string imageName, string uploadsFolder);
    Task<(string imageFileName, string thumbnailFileName)> ProcessImageAsync(IFormFile imageFile, string uploadsFolder);
}

public class ImageStorageHandler : IImageStorageHandler
{
    private readonly IDirectoryManager _directoryManager;

    public ImageStorageHandler(IDirectoryManager directoryManager)
    {
        _directoryManager = directoryManager;
    }

    /// <summary>
    /// Procesa una imagen desde un byte array: la guarda y genera un thumbnail.
    /// Usado para cargas masivas (bulk upload).
    /// </summary>
    public async Task<(string imageFileName, string thumbnailFileName)> ProcessImageAsync(
        byte[] imageBytes, string imageName, string uploadsFolder)
    {
        _directoryManager.EnsureDirectoryExists(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{imageName}.jpg";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
        var thumbnailFileName = $"thumb_{Guid.NewGuid()}.png";
        var thumbnailPath = Path.Combine(uploadsFolder, thumbnailFileName);

        // Save original image
        await File.WriteAllBytesAsync(filePath, imageBytes);

        // Generate thumbnail
        using (var image = Image.Load(filePath))
        {
            image.Mutate(x => x.Resize(new Size(100, 100)));
            using (var thumbStream = new FileStream(thumbnailPath, FileMode.Create))
            {
                image.Save(thumbStream, new PngEncoder());
            }
        }

        return (uniqueFileName, thumbnailFileName);
    }

    /// <summary>
    /// Procesa una imagen desde un IFormFile: la guarda y genera un thumbnail.
    /// Usado para uploads vía API.
    /// </summary>
    public async Task<(string imageFileName, string thumbnailFileName)> ProcessImageAsync(
        IFormFile imageFile, string uploadsFolder)
    {
        _directoryManager.EnsureDirectoryExists(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
        var thumbnailFileName = $"thumb_{Guid.NewGuid()}.png";
        var thumbnailPath = Path.Combine(uploadsFolder, thumbnailFileName);

        // Save original image
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        // Generate thumbnail
        using (var image = Image.Load(filePath))
        {
            image.Mutate(x => x.Resize(new Size(100, 100)));
            using (var thumbStream = new FileStream(thumbnailPath, FileMode.Create))
            {
                image.Save(thumbStream, new PngEncoder());
            }
        }

        return (uniqueFileName, thumbnailFileName);
    }
}

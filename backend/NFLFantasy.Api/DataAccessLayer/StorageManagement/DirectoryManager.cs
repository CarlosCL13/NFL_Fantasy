namespace NFLFantasy.Api.DataAccessLayer.StorageManagement;

/// <summary>
/// Manages directory paths and operations for the application
/// </summary>
public class DirectoryManager
{
    private readonly IWebHostEnvironment _environment;

    public DirectoryManager(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    // NFL Players directories
    public string GetNflPlayersImagesPath() => Path.Combine(_environment.WebRootPath, "images", "nflplayers");
    public string GetNflPlayersUploadsPath() => Path.Combine(_environment.WebRootPath, "uploads");
    public string GetNflPlayersProcessedPath() => Path.Combine(_environment.WebRootPath, "processed");

    // NFL Teams directories
    public string GetNflTeamsImagesPath() => Path.Combine(_environment.WebRootPath, "images", "nflteams");

    // Users directories
    public string GetUsersImagesPath() => Path.Combine(_environment.WebRootPath, "images", "users");

    /// <summary>
    /// Ensures a directory exists, creating it if necessary
    /// </summary>
    public void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// Generates a unique filename with timestamp
    /// </summary>
    public string GenerateUniqueFileName(string baseName, string extension)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        return $"{baseName}__{timestamp}{extension}";
    }

    /// <summary>
    /// Generates a unique filename with timestamp and optional error suffix
    /// </summary>
    public string GenerateUniqueFileName(string baseName, string extension, bool hasError)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var errorSuffix = hasError ? "_ERROR" : "";
        return $"{baseName}__{timestamp}{errorSuffix}{extension}";
    }
}

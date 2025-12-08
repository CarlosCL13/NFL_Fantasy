namespace NFLFantasy.Api.DataAccessLayer.StorageManagement;

/// <summary>
/// Maneja la creación y verificación de directorios para almacenamiento de imágenes y otros archivos.
/// </summary>
public interface IDirectoryManager
{
    string GetNflPlayersImagesPath();
    string GetNflPlayersUploadsPath();
    string GetNflPlayersProcessedPath();
    string GetNflTeamsImagesPath();
    string GetUsersImagesPath();
    void EnsureDirectoryExists(string path);
    void EnsureAllNflPlayersDirectoriesExist();
    void EnsureAllNflTeamsDirectoriesExist();
    void EnsureAllUsersDirectoriesExist();
    string GenerateUniqueFileName(string baseName, string extension);
    string GenerateUniqueFileName(string baseName, string extension, bool hasError);
}

/// <summary>
/// Implementación del manejador de directorios.
/// </summary>
public class DirectoryManager : IDirectoryManager
{
    private readonly IWebHostEnvironment _environment;

    public DirectoryManager(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    // NFL Players directorios
    public string GetNflPlayersImagesPath() => Path.Combine(_environment.WebRootPath, "images", "nflplayers");
    public string GetNflPlayersUploadsPath() => Path.Combine(_environment.WebRootPath, "uploads");
    public string GetNflPlayersProcessedPath() => Path.Combine(_environment.WebRootPath, "processed");

    // NFL Teams directorios
    public string GetNflTeamsImagesPath() => Path.Combine(_environment.WebRootPath, "images", "nflteams");

    // Users directorios
    public string GetUsersImagesPath() => Path.Combine(_environment.WebRootPath, "images", "users");

    /// <summary>
    /// Asegura que un directorio exista, creándolo si es necesario
    /// </summary>
    public void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// Asegura que todos los directorios necesarios para los jugadores NFL existan.
    /// </summary>
    public void EnsureAllNflPlayersDirectoriesExist()
    {
        EnsureDirectoryExists(GetNflPlayersImagesPath());
        EnsureDirectoryExists(GetNflPlayersUploadsPath());
        EnsureDirectoryExists(GetNflPlayersProcessedPath());
    }

    /// <summary>
    /// Asegura que todos los directorios necesarios para los equipos NFL existan.
    /// </summary>
    public void EnsureAllNflTeamsDirectoriesExist()
    {
        EnsureDirectoryExists(GetNflTeamsImagesPath());
    }

    /// <summary>
    /// Asegura que todos los directorios necesarios para los usuarios existan.
    /// </summary>
    public void EnsureAllUsersDirectoriesExist()
    {
        EnsureDirectoryExists(GetUsersImagesPath());
    }

    /// <summary>
    /// Genera un nombre de archivo único con marca de tiempo
    /// </summary>
    public string GenerateUniqueFileName(string baseName, string extension)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        return $"{baseName}__{timestamp}{extension}";
    }

    /// <summary>
    /// Genera un nombre de archivo único con marca de tiempo y sufijo de error opcional
    /// </summary>
    public string GenerateUniqueFileName(string baseName, string extension, bool hasError)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var errorSuffix = hasError ? "_ERROR" : "";
        return $"{baseName}__{timestamp}{errorSuffix}{extension}";
    }
}

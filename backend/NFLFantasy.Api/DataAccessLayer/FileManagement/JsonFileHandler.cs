namespace NFLFantasy.Api.DataAccessLayer.FileManagement;

/// <summary>
/// Handles JSON file operations including moving to processed folder
/// </summary>
public class JsonFileHandler
{
    private readonly StorageManagement.DirectoryManager _directoryManager;

    public JsonFileHandler(StorageManagement.DirectoryManager directoryManager)
    {
        _directoryManager = directoryManager;
    }

    /// <summary>
    /// Moves a JSON file to the processed folder with the appropriate format.
    /// </summary>
    /// <param name="originalFilePath">Original file path</param>
    /// <param name="processedFolder">Destination folder</param>
    /// <param name="hasErrors">If true, adds _ERROR suffix</param>
    /// <returns>Tuple with (success, processedPath, errorMessage)</returns>
    public (bool success, string? processedPath, string? errorMessage) MoveToProcessedFolder(
        string originalFilePath, string processedFolder, bool hasErrors = false)
    {
        try
        {
            _directoryManager.EnsureDirectoryExists(processedFolder);
            
            var originalName = Path.GetFileNameWithoutExtension(originalFilePath);
            var processedFileName = _directoryManager.GenerateUniqueFileName(originalName, ".json", hasErrors);
            var processedPath = Path.Combine(processedFolder, processedFileName);
            
            if (File.Exists(originalFilePath))
            {
                File.Move(originalFilePath, processedPath);
                return (true, processedPath, null);
            }
            
            return (false, null, "El archivo original no existe.");
        }
        catch (Exception ex)
        {
            return (false, null, $"Error al mover archivo: {ex.Message}");
        }
    }
}

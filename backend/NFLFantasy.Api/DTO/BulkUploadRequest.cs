using Microsoft.AspNetCore.Http;

namespace NFLFantasy.Api.DTO
{   
    /// <summary>
    /// DTO para la carga masiva de datos mediante un archivo.
    /// </summary>
    public class BulkUploadRequest
    {
        public IFormFile? File { get; set; }
    }
}

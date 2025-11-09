using Microsoft.AspNetCore.Http;

namespace NFLFantasy.Api.DTO
{
    public class BulkUploadRequest
    {
        public IFormFile File { get; set; }
    }
}

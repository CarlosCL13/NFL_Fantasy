namespace NFLFantasy.Api.DTO
{
    public class CreatePlayerNewsDto
    {
        public int PlayerId { get; set; }
        public string Texto { get; set; } = string.Empty;
        public bool IsLesion { get; set; }
        public string? Resumen { get; set; }
        public int? DesignacionId { get; set; }
    }
}

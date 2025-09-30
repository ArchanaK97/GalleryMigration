namespace GalleryImporter.Models
{
    public class ImportResult
    {
        public string SitecoreId { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public string Status { get; set; } = String.Empty; // Created, Failed, Duplicate, Simulated
        public string Message { get; set; } = String.Empty;
    }
}

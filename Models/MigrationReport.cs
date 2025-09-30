namespace GalleryImporter.Models
{
    public class MigrationReport
    {
        public int TotalProcessed { get; set; }
        public int Created { get; set; }
        public int Failed { get; set; }
        public int Duplicates { get; set; }
        public List<ItemReport> Items { get; set; } = new List<ItemReport>();
    }

    public class ItemReport
    {
        public string FileName { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public int SitecoreId { get; set; }
        public string Status { get; set; } = String.Empty; // "Created", "Failed", "Duplicate"
        public string Message { get; set; } = String.Empty;
        public int? HttpStatusCode { get; set; }
        public string ResponseContent { get; set; } = String.Empty;
    }
}

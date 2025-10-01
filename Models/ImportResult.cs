namespace GalleryImporter.Models
{
    public class ImportResult
    {
        public int TotalCountPass { get; set; } = 0;
        public int TotalCountFail { get; set; } = 0;
        public int TotalCountDuplicate { get; set; } = 0;
        public List<ImportResultProperty> PassData { get; set; } = new List<ImportResultProperty>();
        public List<ImportResultProperty> FailData { get; set; } = new List<ImportResultProperty>();
        public List<ImportResultProperty> DuplicateData { get; set; } = new List<ImportResultProperty>();

    }
    public class ImportResultProperty
    {
       public string SitecoreId { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public string Status { get; set; } = String.Empty; // Created, Failed, Duplicate, Simulated
        public string Message { get; set; } = String.Empty;
       

    }
}

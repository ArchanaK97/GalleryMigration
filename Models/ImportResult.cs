namespace GalleryImporter.Models
{
    public class ImportResult : GalleryItem
    {
       public MigrationResults migrationResults {  get; set; } = new MigrationResults();

    }
    public class MigrationResults
    {
        public string status { get; set; } = String.Empty; // Created, Failed, Duplicate, Simulated
        public string message { get; set; } = String.Empty;
       

    }
}

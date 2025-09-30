namespace GalleryImporter.Models
{
    public class ContentHubEntity
    {
        public string entityDefinition { get; set; } = String.Empty;
        public Dictionary<string, object> properties { get; set; } = new();
        public Dictionary<string, object> relations { get; set; } = new();
        public Dictionary<string, List<ContentHubEntity>> children { get; set; } = new();
    }
}

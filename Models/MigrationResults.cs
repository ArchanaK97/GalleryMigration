namespace GalleryImporter.Models
{
    using System.Text.Json.Serialization;

    public enum MigrationResultStatus
    {
        NotProcessed,
        Created,
        Duplicate,
        Failed,
        Simulated
    }

    public class MigrationResults
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MigrationResultStatus Status { get; set; } = MigrationResultStatus.NotProcessed;
        public List<string> Messages { get; set; } = new List<string>();
    }
}

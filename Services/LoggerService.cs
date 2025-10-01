namespace GalleryImporter.Services
{
    using GalleryImporter.Models;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class LoggerService
    {
        public async Task LogResultsAsync(List<GalleryItem> processedItems)
        {
            // Convert to JSON string
            string json = JsonSerializer.Serialize(processedItems, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            // Save to file in root path
            string currentDirectory = Directory.GetCurrentDirectory();
            string? rootPath = Directory.GetParent(currentDirectory)?.Parent?.Parent?.FullName;
            if (rootPath == null)
            {
                throw new Exception("Could not determine root path.");
            }
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            string filePath = Path.Combine(rootPath, "Export", "ImportResult.json");

            await File.WriteAllTextAsync(filePath, json);

            Console.WriteLine($"Import result saved to: {filePath}");
        }
    }
}

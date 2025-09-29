using GalleryImporter.Services;
using GalleryImporter.Models;

class Program
{
    static void Main(string[] args)
    {
        var inputFolder = "Input"; // put gallery.json files here
        var logFile = "import_log.txt";
        var apiUrl = "";
        var token = "";

        var _galleryRepository = new GalleryRepository(inputFolder);
        var _contentHubService = new ContentHubService(apiUrl, token);
        var _logger = new LoggerService(logFile);

        var dataToImport = _galleryRepository.GetDataToImport();
        var importResult = _contentHubService.Import(dataToImport);
        _logger.LogResults(importResult);

        Console.WriteLine("Import complete. Check log file for details.");
    }
}

using GalleryImporter.Models;
using GalleryImporter.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stylelabs.M.Sdk.WebClient;
using Stylelabs.M.Sdk.WebClient.Authentication;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        var inputFolder = "..\\..\\..\\Input"; // put gallery.json files here
        var logFile = "import_log.txt";
        var apiUrl = "";
        var token = "";


        // Content Hub endpoint (your tenant)
        var endpoint = new Uri("https://mhc-q-002.sitecorecontenthub.cloud/");

        // load from env or secure store
        var oauth = new OAuthClientCredentialsGrant
        {
            //ClientId = Environment.GetEnvironmentVariable("Client_ID"),
           // ClientSecret = Environment.GetEnvironmentVariable("Client_Secret")

            ClientId = "GalleryImport",
            ClientSecret = "{£3\\1luDH8j%h(KS"
        };

        IWebMClient mClient = MClientFactory.CreateMClient(endpoint, oauth);

        await mClient.TestConnectionAsync(); // validate
        Console.WriteLine("Connected!");



        var _galleryRepository = new GalleryRepository(inputFolder);
        var _contentHubService = new ContentHubService(mClient);
       // var _logger = new LoggerService(logFile);

        var dataToImport = _galleryRepository.GetDataToImport();
        var importResult = await _contentHubService.ImportAsync(dataToImport);
        // Convert to JSON string
        string json = System.Text.Json.JsonSerializer.Serialize(importResult, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        // Save to file in root path
        string rootPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        string filePath = Path.Combine(rootPath, "Export", "ImportResult.json");

        await File.WriteAllTextAsync(filePath, json);

        Console.WriteLine($"Import result saved to: {filePath}");

        Console.WriteLine("Import complete. Check log file for details.");
    }
}

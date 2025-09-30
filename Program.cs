using GalleryImporter.Models;
using GalleryImporter.Services;
using Stylelabs.M.Sdk.WebClient;
using Stylelabs.M.Sdk.WebClient.Authentication;

class Program
{
    static async Task Main(string[] args)
    {
        var inputFolder = "Input"; // put gallery.json files here
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
        var _logger = new LoggerService(logFile);

        var dataToImport = _galleryRepository.GetDataToImport();
        var importResult = await _contentHubService.ImportAsync(dataToImport);
       // _logger.LogResults(importResult);

        Console.WriteLine("Import complete. Check log file for details.");
    }
}

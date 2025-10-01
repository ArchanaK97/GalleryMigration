using GalleryImporter.Services;
using Stylelabs.M.Sdk.WebClient;
using Stylelabs.M.Sdk.WebClient.Authentication;

class Program
{
    static async Task Main(string[] args)
    {
        var inputFolder = "..\\..\\..\\Input"; // put gallery.json files here
        var apiUrl = "https://mhc-q-002.sitecorecontenthub.cloud/";
        var token = "{£3\\1luDH8j%h(KS";

        // Content Hub endpoint (your tenant)
        var endpoint = new Uri(apiUrl);

        // load from env or secure store
        var oauth = new OAuthClientCredentialsGrant
        {
            ClientId = "GalleryImport",
            ClientSecret = token
        };

        IWebMClient mClient = MClientFactory.CreateMClient(endpoint, oauth);

        await mClient.TestConnectionAsync(); // validate
        Console.WriteLine($"Test Connection to Content Hub {endpoint.AbsoluteUri} succeeded!");

        var _galleryRepository = new GalleryRepository(inputFolder);
        var _contentHubService = new ContentHubService(mClient);
        var _logger = new LoggerService();

        Console.WriteLine("Reading data from input folder...");
        var dataToImport = _galleryRepository.GetDataToImport();
        Console.WriteLine("Data read complete.");
        Console.WriteLine();
        Console.WriteLine($"Importing {dataToImport.Count} items to Content Hub...");
        var importResult = _contentHubService.ImportAsync(dataToImport).Result;
        Console.WriteLine($"Processed {importResult.Count} items.");
        Console.WriteLine();
        Console.WriteLine("Saving import results to log file...");
        _logger.LogResultsAsync(importResult).Wait();
        Console.WriteLine("Import complete. Check log file for details.");
    }
}

using RestSharp;
using Newtonsoft.Json;
using GalleryImporter.Models;

namespace GalleryImporter.Services
{
    public class ContentHubService
    {
        private readonly string _apiUrl;
        private readonly string _token;

        public ContentHubService(string apiUrl, string token)
        {
            _apiUrl = apiUrl;
            _token = token;
        }

        public List<ImportResult> Import(List<GalleryItem> items)
        {
            var results = new List<ImportResult>();

            foreach (var item in items)
            {
               // var entity = Mapper.MapToContentHub(item);

               // Console.WriteLine("Entity name", entity.properties.ContainsKey("Name"));
                //Skip API call, just simulate a result
                results.Add(new ImportResult
                {
                    SitecoreId = item.sitecoreId,
                    Name = item.galleryItemName
                });
            }

            return results;
        }

    }
}
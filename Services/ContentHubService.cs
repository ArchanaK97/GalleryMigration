using GalleryImporter.Models;
using Stylelabs.M.Sdk.Contracts.Base;
using Stylelabs.M.Sdk.WebClient;
using static Stylelabs.M.Sdk.Defaults;
using static Stylelabs.M.Sdk.Errors;


namespace GalleryImporter.Services
{
    public class ContentHubService
    {
        private readonly IWebMClient _mClient;

        public ContentHubService(IWebMClient mClient)
        {
            _mClient = mClient;
        }

        public async Task<List<ImportResult>> ImportAsync(List<GalleryItem> items)
        {
            long idd = 7599739;
            var results = new List<ImportResult>();
            var definition = await _mClient.EntityDefinitions.GetAsync("MH.AssetGallery");
            var getentity = await _mClient.Entities.GetAsync(idd);
            Console.WriteLine($"Definition: {definition.Name}");
            foreach (var member in definition.MemberGroups)
            {
                Console.WriteLine($"{member.Name} (type: {member.MemberDefinitions})");
            }
            var entity = await _mClient.EntityFactory.CreateAsync("MH.AssetGallery");
            entity.SetPropertyValue("Title","TestGalleryMigration");
           // entity.SetPropertyValue("MetroAreaIDs", "a1Icw00000088ejEAA");
            entity.SetPropertyValue("MetroAreaIDs", "a1IC0000002KG0SMAW");
     
             var savedEntity = await _mClient.Entities.SaveAsync(entity);

            //foreach (var item in items)
            //{
            //    // Create a new Gallery entity
            //    var entity = await _mClient.EntityFactory.CreateAsync("MH.AssetGallery");

            //    // Set properties (adjust to your schema)
            //    entity.SetPropertyValue("Title", item.galleryItemName);
            //    //entity.SetPropertyValue("SitecoreId", item.sitecoreId);

            //    // Save to Content Hub
            //   // var savedEntity = await _mClient.Entities.SaveAsync(entity);

            //    results.Add(new ImportResult
            //    {
            //        SitecoreId = item.sitecoreId,
            //        Name = item.galleryItemName
            //    });
            //}

            return results;
        }

    }
}
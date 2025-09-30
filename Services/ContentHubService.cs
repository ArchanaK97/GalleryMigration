namespace GalleryImporter.Services
{
    using GalleryImporter.Models;
    using Stylelabs.M.Framework.Essentials.LoadConfigurations;
    using Stylelabs.M.Framework.Essentials.LoadOptions;
    using Stylelabs.M.Sdk.Contracts.Base;
    using Stylelabs.M.Sdk.WebClient;

    public class ContentHubService
    {
        private readonly IWebMClient _mClient;

        public ContentHubService(IWebMClient mClient)
        {
            _mClient = mClient;
        }

        public async Task<List<ImportResult>> ImportAsync(List<GalleryItem> items)
        {
            var results = new List<ImportResult>();
            var definition = await _mClient.EntityDefinitions.GetAsync("MH.AssetGallery");
            Console.WriteLine($"Definition: {definition.Name}");
            foreach (var member in definition.MemberGroups)
            {
                Console.WriteLine($"{member.Name} (type: {member.MemberDefinitions})");
            }
            var entity = await _mClient.EntityFactory.CreateAsync("MH.AssetGallery");
            entity.SetPropertyValue("Title","TestGalleryMigration");

            var metroArea = await _mClient.Entities.GetAsync("ux8xh4j1pUGBz8Iw6TNTpw", new EntityLoadConfiguration
            {
                RelationLoadOption = RelationLoadOption.All
            });

            var galleriesToMetroAreas = entity.GetRelation< IChildToManyParentsRelation>("GalleriesToMetroAreas");
            if (metroArea?.Id != null)
            {
                galleriesToMetroAreas.Add(metroArea.Id.Value);
            }

            var savedEntity = await _mClient.Entities.SaveAsync(entity);

            return results;
        }

    }
}
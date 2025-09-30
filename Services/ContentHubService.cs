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
            foreach (var item in items)
            {
                var entity = await _mClient.EntityFactory.CreateAsync("MH.AssetGallery");
                entity.SetPropertyValue("Title", item.galleryTitle);
                entity.SetPropertyValue("Description", item.galleryDescription);
                foreach (var relatedmetroarea in item.relatedMetroAreas)
                {
                    var metroArea = await _mClient.Entities.GetAsync(relatedmetroarea.contentHubEntityIdentifier, new EntityLoadConfiguration
                    {
                        RelationLoadOption = RelationLoadOption.All
                    });
                    var galleriesToMetroAreas = entity.GetRelation<IChildToManyParentsRelation>("GalleriesToMetroAreas");
                    if (metroArea?.Id != null)
                    {
                        galleriesToMetroAreas.Add(metroArea.Id.Value);
                    }
                }
                foreach (var lotItem in item.relatedLots)
                {
                    var lot = await _mClient.Entities.GetAsync(lotItem.contentHubEntityIdentifier, new EntityLoadConfiguration
                    {
                       RelationLoadOption = RelationLoadOption.All
                    });
                    var galleriesToLots = entity.GetRelation<IChildToManyParentsRelation>("GalleriesToLots");
                    if (lot?.Id != null)
                    {
                        galleriesToLots.Add(lot.Id.Value);
                    }
                }
                foreach (var floorplanItem in item.relatedFloorplans)
                {
                    var floorplan = await _mClient.Entities.GetAsync(floorplanItem.contentHubEntityIdentifier, new EntityLoadConfiguration
                    {
                        RelationLoadOption = RelationLoadOption.All
                    });
                    var galleriesToPlans = entity.GetRelation<IChildToManyParentsRelation>("GalleriesToPlans");
                    if (floorplan?.Id != null)
                    {
                        galleriesToPlans.Add(floorplan.Id.Value);
                    }
                }
                foreach (var relatedcommunitysheet in item.relatedCommunitySheets)
                {
                    var communitySheet = await _mClient.Entities.GetAsync(relatedcommunitysheet.contentHubEntityIdentifier, new EntityLoadConfiguration
                    {
                        RelationLoadOption = RelationLoadOption.All
                    });
                    var galleriesToCommunitySheets = entity.GetRelation<IChildToManyParentsRelation>("GalleriesToCommunitySheets");
                    if (communitySheet?.Id != null)
                    {
                        galleriesToCommunitySheets.Add(communitySheet.Id.Value);
                    }
                }
                //foreach (var sliedItem in item.slides)
                //{
                //    var slide = await _mClient.Entities.GetAsync(sliedItem.slideTitle, new EntityLoadConfiguration
                //    {
                //        RelationLoadOption = RelationLoadOption.All
                //    });
                //    var assetGalleriesToAssetGallerySlides = entity.GetRelation<IParentToManyChildrenRelation>("AssetGalleriesToAssetGallerySlides");
                //    if (slide?.Id != null)
                //    {
                //        assetGalleriesToAssetGallerySlides.Add(slide.Id.Value);
                //    }
                //}
                var savedEntity = await _mClient.Entities.SaveAsync(entity);
                break;

            }



            return results;
        }

    }
}
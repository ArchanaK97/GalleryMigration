namespace GalleryImporter.Services
{
    using GalleryImporter.Models;
    using Stylelabs.M.Framework.Essentials.LoadConfigurations;
    using Stylelabs.M.Framework.Essentials.LoadOptions;
    using Stylelabs.M.Sdk.Contracts.Base;
    using Stylelabs.M.Sdk.WebClient;
    using System.Threading.Tasks;

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
                foreach (var slideItem in item.slides)
                {
                    long? assetReference = null;
                    if (slideItem.useContentHubImage)
                    {
                        var entityId = await GetEntityForUrl(slideItem.contentHubImageUrl);
                        if (!string.IsNullOrEmpty(entityId))
                        {
                            assetReference = long.Parse(entityId)-1; //I added the subtract one because I noticed with the asset I was testing with, the actual id was 1 less than the id emitted by the header value
                        }
                    }
                    else
                    {
                        //TODO: Handle the case where the image is not a Content Hub Asset and needs to be imported and then referenced
                    }

                    if (assetReference != null)
                    {
                        var slide = await _mClient.EntityFactory.CreateAsync("MH.AssetGallerySlide");
                        slide.SetPropertyValue("Caption", slideItem.slideTitle);
                        slide.SetPropertyValue("Description", slideItem.slideCaption);
                        
                        var assetToGallerySlides = slide.GetRelation<IChildToOneParentRelation>("AssetToGallerySlides");
                        assetToGallerySlides.SetId(assetReference.Value);

                        var savedSlide = await _mClient.Entities.SaveAsync(slide);

                        var assetGalleriesToAssetGallerySlides = entity.GetRelation<IParentToManyChildrenRelation>("AssetGalleriesToAssetGallerySlides");
                        assetGalleriesToAssetGallerySlides.Add(savedSlide);
                    }
                }
                var savedEntity = await _mClient.Entities.SaveAsync(entity);
                break;
            }

            return results;
        }

        private async Task<string?> GetEntityForUrl(string contentHubImageUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(contentHubImageUrl);

                    response.EnsureSuccessStatusCode();

                    if (response.Headers.TryGetValues("Entity", out var entityValues))
                    {
                        return entityValues?.ToList()[0];
                    }
                    else
                    {
                        //TODO: Log missing Entity Header
                    }
                }
                catch (Exception ex)
                {
                    //TODO:  Log exception
                }
            }

            return null;
        }
    }
}
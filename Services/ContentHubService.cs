namespace GalleryImporter.Services
{
    using GalleryImporter.Models;
    using Stylelabs.M.Base.Querying;
    using Stylelabs.M.Base.Querying.Filters;
    using Stylelabs.M.Base.Querying.Linq;
    using Stylelabs.M.Framework.Essentials.LoadConfigurations;
    using Stylelabs.M.Framework.Essentials.LoadOptions;
    using Stylelabs.M.Sdk.Contracts.Base;
    using Stylelabs.M.Sdk.WebClient;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
                ////create computed gallery title value
                ImportResult importResult = new ImportResult();
                StringBuilder computedGallerytitle = new StringBuilder();
                computedGallerytitle.Append("Imported:");
                if (item.relatedMetroAreas != null && item.relatedMetroAreas.Count() != 0)
                {
                    computedGallerytitle.Append(item.relatedMetroAreas[0].name);
                    computedGallerytitle.Append(" ");
                }
                if (item.relatedCommunitySheets != null && item.relatedCommunitySheets.Count() != 0)
                {
                    computedGallerytitle.Append(item.relatedCommunitySheets[0].name);
                    computedGallerytitle.Append(" ");
                }
                if (item.relatedFloorplans != null && item.relatedFloorplans.Count() != 0)
                {
                    computedGallerytitle.Append(item.relatedFloorplans[0].name);
                    computedGallerytitle.Append(" ");
                }
                if (item.relatedLots != null && item.relatedLots.Count() != 0)
                {
                    computedGallerytitle.Append(item.relatedLots[0].name);
                }

                ////checking duplicate entity
                var getEntity = await GetGalleryByTitleAsync(computedGallerytitle.ToString());
                if (getEntity != null)
                {
                    // Duplicate entity     
                    importResult.migrationResults.status = "Duplicate";
                    importResult.migrationResults.message = "Duplicate item is trying to import";
                    break;
                }
                else
                {
                    // New entity created
                    var entity = await _mClient.EntityFactory.CreateAsync("MH.AssetGallery");
                    entity.SetPropertyValue("Title", computedGallerytitle.ToString());
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
                                assetReference = long.Parse(entityId) - 1; //I added the subtract one because I noticed with the asset I was testing with, the actual id was 1 less than the id emitted by the header value
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
                    
                    importResult.migrationResults.status = "Pass";
                    importResult.migrationResults.message = "Entity created successfully!";
                   
                }
                //Assign back to export object to export json
                importResult.relatedLots = item.relatedLots;    
                importResult.isPublished = item.isPublished;    
                importResult.galleryTitle = item.galleryTitle;    
                importResult.slides = item.slides;    
                importResult.relatedFloorplans = item.relatedFloorplans;    
                importResult.isFeaturedGallery = item.isFeaturedGallery;    
                importResult.galleryDescription = item.galleryDescription;    
                importResult.galleryItemName = item.galleryItemName;    
                importResult.relatedMetroAreas = item.relatedMetroAreas;    
                importResult.sitecoreId = item.sitecoreId;    
                importResult.relatedCommunitySheets = item.relatedCommunitySheets;    
                results.Add(importResult);
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
        private async Task<IEntity> GetGalleryByTitleAsync(string title)
        {
            // Create a filter: DefinitionName = MH.AssetGallery AND Title = {title}
            var query = Query.CreateQuery(entities =>
   from e in entities
   where e.Property("Title") == title
   select e);
            var result = await _mClient.Querying.QueryAsync(query).ConfigureAwait(false);
            if (result.Items.Any())
            {
                var id = result.Items.First().Id;
                return await _mClient.Entities.GetAsync(id.Value);
            }

            return null;
        }

    }
}
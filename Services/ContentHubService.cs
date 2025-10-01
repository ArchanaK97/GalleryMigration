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
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;
    using System.Xml.Linq;
    using static Stylelabs.M.Sdk.Errors;

    /// <summary>
    /// 
    /// Notes:  Namespace for list of filter types: Stylelabs.M.Base.Querying.Filters.DefinitionQueryFilter
    /// </summary>
    public class ContentHubService
    {
        private readonly IWebMClient _mClient;

        public ContentHubService(IWebMClient mClient)
        {
            _mClient = mClient;
        }

        public async Task<List<GalleryItem>> ImportAsync(List<GalleryItem> items)
        {
            foreach (var item in items)
            {
                Console.WriteLine($"Processing item: {item.ComputedTitle}");
                ////checking duplicate entity
                var getEntity = await GetGalleryByTitleAsync(item.ComputedTitle);
                if (getEntity != null)
                {
                    // Duplicate entity     
                    item.migrationResults.Status = MigrationResultStatus.Duplicate;
                    item.migrationResults.Messages.Add("Duplicate item is trying to import");
                    continue;
                }
                else
                {
                    try {
                        // New entity created
                        var newAssetGallery = await _mClient.EntityFactory.CreateAsync("MH.AssetGallery");
                        newAssetGallery.SetPropertyValue("Title", item.ComputedTitle);
                        newAssetGallery.SetPropertyValue("Description", item.galleryDescription);

                        ProcessRelatedMetroAreas(item, newAssetGallery);
                        ProcessRelatedLots(item, newAssetGallery);
                        ProcessRelatedFloorplans(item, newAssetGallery);
                        ProcessRelatedCommunitySheets(item, newAssetGallery);
                        var slidesToCreate = ProcessSlides(item, newAssetGallery);

                        if (item.migrationResults.Status == MigrationResultStatus.NotProcessed)
                        {
                            //foreach (var slidetoCreate in slidesToCreate)
                            //{
                            //    var savedSlide = await _mClient.Entities.SaveAsync(slidetoCreate);

                            //    var assetGalleriesToAssetGallerySlides = entity.GetRelation<IParentToManyChildrenRelation>("AssetGalleriesToAssetGallerySlides");
                            //    assetGalleriesToAssetGallerySlides.Add(savedSlide);
                            //}

                            //var savedEntity = await _mClient.Entities.SaveAsync(entity);

                            item.migrationResults.Status = MigrationResultStatus.Created;
                            item.migrationResults.Messages.Add("Entity created successfully!");
                        }
                    }
                    catch (Exception ex)
                    {
                        item.migrationResults.Status = MigrationResultStatus.Failed;
                        item.migrationResults.Messages.Add($"Error: {ex.Message}");
                    }
                }

                Console.WriteLine($"Finished processing item: {item.ComputedTitle} with status {item.migrationResults.Status}");
            }
            return items;
        }

        private void ProcessRelatedMetroAreas(GalleryItem item, IEntity entity)
        {
            foreach (var relatedmetroarea in item.relatedMetroAreas)
            {
                var metroArea = _mClient.Entities.GetAsync(relatedmetroarea.contentHubEntityIdentifier, new EntityLoadConfiguration
                {
                    RelationLoadOption = RelationLoadOption.All
                }).Result;

                if (metroArea?.Id == null)
                {
                    item.migrationResults.Status = MigrationResultStatus.Failed;
                    item.migrationResults.Messages.Add($"Error: Could not find Metro Area with identifier {relatedmetroarea.contentHubEntityIdentifier}.");
                    continue;
                }

                var galleriesToMetroAreas = entity.GetRelation<IChildToManyParentsRelation>("GalleriesToMetroAreas");
                galleriesToMetroAreas.Add(metroArea.Id.Value);
            }
        }

        private void ProcessRelatedLots(GalleryItem item, IEntity entity)
        {
            foreach (var lotItem in item.relatedLots)
            {
                var lot = _mClient.Entities.GetAsync(lotItem.contentHubEntityIdentifier, new EntityLoadConfiguration
                {
                    RelationLoadOption = RelationLoadOption.All
                }).Result;
                if (lot?.Id == null)
                {
                    item.migrationResults.Status = MigrationResultStatus.Failed;
                    item.migrationResults.Messages.Add($"Error: Could not find Lot with identifier {lotItem.contentHubEntityIdentifier}.");
                    continue;
                }
                var galleriesToLots = entity.GetRelation<IChildToManyParentsRelation>("GalleriesToLots");
                galleriesToLots.Add(lot.Id.Value);
            }
        }

        private void ProcessRelatedFloorplans(GalleryItem item, IEntity entity)
        {
            foreach (var floorplanItem in item.relatedFloorplans)
            {
                var floorplan = _mClient.Entities.GetAsync(floorplanItem.contentHubEntityIdentifier, new EntityLoadConfiguration
                {
                    RelationLoadOption = RelationLoadOption.All
                }).Result;
                if (floorplan?.Id == null)
                {
                    item.migrationResults.Status = MigrationResultStatus.Failed;
                    item.migrationResults.Messages.Add($"Error: Could not find Floorplan with identifier {floorplanItem.contentHubEntityIdentifier}.");
                    continue;
                }
                var galleriesToPlans = entity.GetRelation<IChildToManyParentsRelation>("GalleriesToPlans");
                galleriesToPlans.Add(floorplan.Id.Value);
            }
        }

        private void ProcessRelatedCommunitySheets(GalleryItem item, IEntity entity)
        {
            foreach (var relatedcommunitysheet in item.relatedCommunitySheets)
            {
                var communitySheet = _mClient.Entities.GetAsync(relatedcommunitysheet.contentHubEntityIdentifier, new EntityLoadConfiguration
                {
                    RelationLoadOption = RelationLoadOption.All
                }).Result;
                if (communitySheet?.Id == null)
                {
                    item.migrationResults.Status = MigrationResultStatus.Failed;
                    item.migrationResults.Messages.Add($"Error: Could not find Community Sheet with identifier {relatedcommunitysheet.contentHubEntityIdentifier}.");
                    continue;
                }
                var galleriesToCommunitySheets = entity.GetRelation<IChildToManyParentsRelation>("GalleriesToCommunitySheets");
                galleriesToCommunitySheets.Add(communitySheet.Id.Value);
            }
        }

        private List<IEntity> ProcessSlides(GalleryItem item, IEntity entity)
        {
            List<IEntity> slidesToCreate = new List<IEntity>();
            foreach (var slideItem in item.slides)
            {
                long? assetReference = null;
                if (slideItem.useContentHubImage)
                {
                    var entityId = GetEntityForUrl(slideItem.contentHubImageUrl).Result;

                    if (string.IsNullOrEmpty(entityId))
                    {
                        item.migrationResults.Status = MigrationResultStatus.Failed;
                        item.migrationResults.Messages.Add($"Error: Slide with index {item.slides.IndexOf(slideItem)} could not find a valid Content Hub Asset for URL {slideItem.contentHubImageUrl}.");
                        continue;
                    }

                    assetReference = long.Parse(entityId);
                }
                else
                {
                    assetReference = long.Parse(slideItem.slideImage);
                }

                if (assetReference == null)
                {
                    item.migrationResults.Status = MigrationResultStatus.Failed;
                    item.migrationResults.Messages.Add($"Error: Slide with index {item.slides.IndexOf(slideItem)} could not find a valid Content Hub Asset reference.");
                    continue;
                }

                var slide = _mClient.EntityFactory.CreateAsync("MH.AssetGallerySlide").Result;
                slide.SetPropertyValue("Caption", slideItem.slideTitle);
                slide.SetPropertyValue("Description", slideItem.slideCaption);

                var assetToGallerySlides = slide.GetRelation<IChildToOneParentRelation>("AssetToGallerySlides");
                if (assetToGallerySlides == null)
                {
                    item.migrationResults.Status = MigrationResultStatus.Failed;
                    item.migrationResults.Messages.Add($"Error: Slide with index {item.slides.IndexOf(slideItem)} could not establish Asset to Gallery Slide relation.");
                    continue;
                }
                assetToGallerySlides.SetId(assetReference.Value);

                slidesToCreate.Add(slide);
            }

            return slidesToCreate;
        }

        private async Task<string?> GetEntityForUrl(string contentHubImageUrl)
        {
            var uri = new Uri(contentHubImageUrl);

            string lastSegment = uri.Segments[^1];
            string vParam = HttpUtility.ParseQueryString(uri.Query).Get("v") ?? string.Empty;
            
            var definitionFilter = new DefinitionQueryFilter { Name = "M.PublicLink" };
            var relativeUrlFilter = new PropertyQueryFilter
            {
                Property = "RelativeUrl",
                DataType = FilterDataType.String,
                Operator = ComparisonOperator.Equals,
                Value = lastSegment
            };
            var versionFilter = new PropertyQueryFilter
            {
                Property = "VersionHash",
                DataType = FilterDataType.String,
                Operator = ComparisonOperator.Equals,
                Value = vParam
            };
            var compositeFilter = new CompositeQueryFilter
            {
                Children = new List<QueryFilter>([definitionFilter, relativeUrlFilter, versionFilter])
            };

            var publicLinkEntity = await _mClient.Querying.QueryAsync(new Query
            {
                Filter = compositeFilter,
            }, new EntityLoadConfiguration
            {
                PropertyLoadOption = PropertyLoadOption.All,
                RelationLoadOption = RelationLoadOption.All
            });

            var publicLink = publicLinkEntity.Items.FirstOrDefault();
            var assetToPublicLink = publicLink?.GetRelation<IChildToManyParentsRelation>("AssetToPublicLink");
            var assetId = assetToPublicLink?.Parents.FirstOrDefault();

            return assetId.ToString();
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
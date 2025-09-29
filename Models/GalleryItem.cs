using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalleryImporter.Models
{
    public class GalleryItem
    {
        public MigrationResults migrationResults { get; set; }
        public int sitecoreId { get; set; }
        public string galleryItemName { get; set; }
        public string galleryTitle { get; set; }
        public string galleryDescription { get; set; }
        public bool isPublished { get; set; }
        public List<RelatedFloorplan> relatedFloorplans { get; set; }
        public List<RelatedLot> relatedLots { get; set; }
        public List<RelatedCommunitySheet> relatedCommunitySheets { get; set; }
        public List<RelatedMetroArea> relatedMetroAreas { get; set; }
        public bool isFeaturedGallery { get; set; }
        public List<Slide> slides { get; set; }
    }

    public class MigrationResults
    {
        public string status { get; set; }
        public string message { get; set; }
    }

    public class RelatedFloorplan
    {
        public int sfId { get; set; }
        public string name { get; set; }
        public string contentHubEntityIdentifier { get; set; }
        public int communitySheetSfId { get; set; }
    }

    public class RelatedLot
    {
        public int sfId { get; set; }
        public string address1 { get; set; }
        public string contentHubEntityIdentifier { get; set; }
        public int floorPlanSfId { get; set; }
    }

    public class RelatedCommunitySheet
    {
        public int sfId { get; set; }
        public string name { get; set; }
        public string contentHubEntityIdentifier { get; set; }
    }

    public class RelatedMetroArea
    {
        public int sfId { get; set; }
        public string name { get; set; }
        public string contentHubEntityIdentifier { get; set; }
    }

    public class Slide
    {
        public string slideTitle { get; set; }
        public string slideCaption { get; set; }
        public string slideImage { get; set; }
        public bool useContentHubImage { get; set; }
        public string contentHubImageUrl { get; set; }
        public int slideIndex { get; set; }
    }
}

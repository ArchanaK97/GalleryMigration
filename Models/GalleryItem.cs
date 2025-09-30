using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalleryImporter.Models
{
    public class GalleryItem
    {
        public List<Relatedlot> relatedLots { get; set; }
        public bool isPublished { get; set; }
        public string galleryTitle { get; set; }
        public List<Slide> slides { get; set; }
        public List<Relatedfloorplan> relatedFloorplans { get; set; }
        public bool isFeaturedGallery { get; set; }
        public string galleryDescription { get; set; }
        public string galleryItemName { get; set; }
        public List<Relatedmetroarea> relatedMetroAreas { get; set; }
        public string sitecoreId { get; set; }
        public List<Relatedcommunitysheet> relatedCommunitySheets { get; set; }
    }
    public class Relatedlot
    {
        public string address1 { get; set; }
        public string sfId { get; set; }
        public string name { get; set; }
        public string ID { get; set; }
        public string contentHubEntityIdentifier { get; set; }
    }

    public class Slide
    {
        public string contentHubImageUrl { get; set; }
        public string slideIndex { get; set; }
        public bool useContentHubImage { get; set; }
        public string slideCaption { get; set; }
        public string slideTitle { get; set; }
        public string slideImage { get; set; }
    }

    public class Relatedfloorplan
    {
        public string sfId { get; set; }
        public string name { get; set; }
        public string ID { get; set; }
        public string contentHubEntityIdentifier { get; set; }
        public string communitySheetSfId { get; set; }
    }

    public class Relatedmetroarea
    {
        public string sfId { get; set; }
        public string name { get; set; }
        public string contentHubEntityIdentifier { get; set; }
    }

    public class Relatedcommunitysheet
    {
        public string sfId { get; set; }
        public string name { get; set; }
        public string ID { get; set; }
        public string contentHubEntityIdentifier { get; set; }
    }

}


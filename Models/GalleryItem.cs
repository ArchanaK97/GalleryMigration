namespace GalleryImporter.Models
{
    using Remotion.Linq.Utilities;
    using System.Text;

    public class GalleryItem
    {
        public List<Relatedlot> relatedLots { get; set; } = new List<Relatedlot>();
        public bool isPublished { get; set; }
        public string galleryTitle { get; set; } = String.Empty;
        public List<Slide> slides { get; set; } = new List<Slide>();
        public List<Relatedfloorplan> relatedFloorplans { get; set; } = new List<Relatedfloorplan>();
        public bool isFeaturedGallery { get; set; }
        public string galleryDescription { get; set; } = String.Empty;
        public string galleryItemName { get; set; } = String.Empty;
        public List<Relatedmetroarea> relatedMetroAreas { get; set; } = new List<Relatedmetroarea>();
        public string sitecoreId { get; set; } = String.Empty;
        public List<Relatedcommunitysheet> relatedCommunitySheets { get; set; } = new List<Relatedcommunitysheet>();

        public string ComputedTitle
        {
            get
            {
                StringBuilder computedGallerytitle = new StringBuilder();
                computedGallerytitle.Append("Imported: ");

                bool hasIdentifier = false;
                if (relatedMetroAreas != null && relatedMetroAreas.Count() != 0)
                {
                    computedGallerytitle.Append(relatedMetroAreas[0].name);
                    computedGallerytitle.Append(" ");
                    hasIdentifier = true;
                }
                if (relatedCommunitySheets != null && relatedCommunitySheets.Count() != 0)
                {
                    computedGallerytitle.Append(relatedCommunitySheets[0].name);
                    computedGallerytitle.Append(" ");
                    hasIdentifier = true;
                }
                if (relatedFloorplans != null && relatedFloorplans.Count() != 0)
                {
                    computedGallerytitle.Append(relatedFloorplans[0].name);
                    computedGallerytitle.Append(" ");
                    hasIdentifier = true;
                }
                if (relatedLots != null && relatedLots.Count() != 0)
                {
                    computedGallerytitle.Append(relatedLots[0].name);
                    hasIdentifier = true;
                }

                if (!hasIdentifier)
                {
                    computedGallerytitle.Append(galleryItemName);
                }

                return computedGallerytitle.ToString();
            }
        }

        public MigrationResults migrationResults { get; set; } = new MigrationResults();
    }
    public class Relatedlot
    {
        public string address1 { get; set; } = String.Empty;
        public string sfId { get; set; } = String.Empty;
        public string name { get; set; } = String.Empty;
        public string ID { get; set; } = String.Empty;
        public string contentHubEntityIdentifier { get; set; } = String.Empty;
    }

    public class Slide
    {
        public string contentHubImageUrl { get; set; } = String.Empty;
        public string slideIndex { get; set; } = String.Empty;
        public bool useContentHubImage { get; set; }
        public string slideCaption { get; set; } = String.Empty;
        public string slideTitle { get; set; } = String.Empty;
        public string slideImage { get; set; } = String.Empty;
    }

    public class Relatedfloorplan
    {
        public string sfId { get; set; } = String.Empty;
        public string name { get; set; } = String.Empty;
        public string ID { get; set; } = String.Empty;
        public string contentHubEntityIdentifier { get; set; } = String.Empty;
        public string communitySheetSfId { get; set; } = String.Empty;
    }

    public class Relatedmetroarea
    {
        public string sfId { get; set; } = String.Empty;
        public string name { get; set; } = String.Empty;
        public string contentHubEntityIdentifier { get; set; } = String.Empty;
    }

    public class Relatedcommunitysheet
    {
        public string sfId { get; set; } = String.Empty;
        public string name { get; set; } = String.Empty;
        public string ID { get; set; } = String.Empty;
        public string contentHubEntityIdentifier { get; set; } = String.Empty;
    }

}
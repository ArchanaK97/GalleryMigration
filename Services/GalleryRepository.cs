using GalleryImporter.Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace GalleryImporter.Services
{
    public class GalleryRepository
    {
        private readonly string _inputFolder;

        public GalleryRepository(string inputFolder)
        {
            _inputFolder = inputFolder;
        }
        public static string ExtractContentHubId(string imageTag)
        {
            if (string.IsNullOrWhiteSpace(imageTag))
                return string.Empty;

            var match = Regex.Match(imageTag, @"stylelabs-content-id\s*=\s*['""](?<id>\d+)['""]");
            return match.Success ? match.Groups["id"].Value : string.Empty;
        }
        public List<GalleryItem> GetDataToImport()
        {
            var galleries = new List<GalleryItem>();

            foreach (var file in Directory.GetFiles(_inputFolder, "*.json"))
            {
                var json = File.ReadAllText(file);
                var items = JsonConvert.DeserializeObject<List<GalleryItem>>(json);
                if (items != null)
                {
                    foreach (var gallery in items)
                    {
                        if (gallery.slides != null)
                        {
                            foreach (var slide in gallery.slides)
                            {
                                // Extract only the contenthub-id from the image tag
                                slide.slideImage = ExtractContentHubId(slide.slideImage);
                            }
                        }
                    }

                    galleries.AddRange(items);
                }
            }

            return galleries;
        }
    }
    }

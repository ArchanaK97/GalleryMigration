using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalleryImporter.Models
{
    public class ContentHubEntity
    {
        public string entityDefinition { get; set; }
        public Dictionary<string, object> properties { get; set; } = new();
        public Dictionary<string, object> relations { get; set; } = new();
        public Dictionary<string, List<ContentHubEntity>> children { get; set; } = new();
    }
}

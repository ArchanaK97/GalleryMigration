using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalleryImporter.Models
{
    public class ImportResult
    {
        public int SitecoreId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; } // Created, Failed, Duplicate, Simulated
        public string Message { get; set; }
    }
}

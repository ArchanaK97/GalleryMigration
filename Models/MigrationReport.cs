using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalleryImporter.Models
{
    public class MigrationReport
    {
        public int TotalProcessed { get; set; }
        public int Created { get; set; }
        public int Failed { get; set; }
        public int Duplicates { get; set; }
        public List<ItemReport> Items { get; set; } = new();
    }

    public class ItemReport
    {
        public string FileName { get; set; }
        public string Name { get; set; }
        public int SitecoreId { get; set; }
        public string Status { get; set; } // "Created", "Failed", "Duplicate"
        public string Message { get; set; }
        public int? HttpStatusCode { get; set; }
        public string ResponseContent { get; set; }
    }
}

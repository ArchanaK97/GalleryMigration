using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalleryImporter.Models;


namespace GalleryImporter.Services
{
    public class LoggerService
    {
        private readonly string _logFile;

        public LoggerService(string logFile)
        {
            _logFile = logFile;
        }

        public void LogResults(List<ImportResult> results)
        {
            using var writer = new StreamWriter(_logFile, append: true);

            writer.WriteLine($"=== Import Run {DateTime.UtcNow} ===");

            int created = results.Count(r => r.Status == "Created");
            int failed = results.Count(r => r.Status == "Failed");
            int duplicates = results.Count(r => r.Status == "Duplicate");

            writer.WriteLine($"Created: {created}, Failed: {failed}, Duplicates: {duplicates}");

            foreach (var r in results)
            {
                writer.WriteLine($"{r.SitecoreId} | {r.Name} | {r.Status} | {r.Message}");
            }

            writer.WriteLine("========================================");
        }
    }
}

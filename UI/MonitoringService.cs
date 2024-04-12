using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PDFDataExtraction.Core;

namespace PDFDataExtraction
{
    public class MonitoringService : IMonitoringService
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly IConfiguration _configuration;

        public bool IsMonitoring { get; private set; } = false;

        // Constructor that accepts IConfiguration
        public MonitoringService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void StartMonitoring()
        {
            if (IsMonitoring) return;

            // Directly access configuration for paths
            var pdfFolder = _configuration["ApplicationPaths:PdfFolder"] ?? throw new InvalidOperationException("PdfFolder Path not found in appsettings.json");
            var outputFolder = _configuration["ApplicationPaths:OutputFolder"] ?? throw new InvalidOperationException("OutputFolder Path not found in appsettings.json");
            var validFolder = _configuration["ApplicationPaths:ValidFolder"] ?? throw new InvalidOperationException("ValidFolder Path not found in appsettings.json");

            IsMonitoring = true;
            _cts = new CancellationTokenSource(); // Reset the CancellationTokenSource for a new task

            Task.Run(() => Program.MonitorPdfFolder(pdfFolder, outputFolder, validFolder, _cts.Token), _cts.Token);
        }

        public void StopMonitoring()
        {
            if (!IsMonitoring) return;

            _cts.Cancel();
            IsMonitoring = false;
        }
    }
}

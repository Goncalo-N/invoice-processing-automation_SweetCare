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

            IsMonitoring = true;
            _cts = new CancellationTokenSource(); // Reset the CancellationTokenSource for a new task

            Task.Run(() => Program.MonitorPdfFolder(_cts.Token), _cts.Token);
        }

        public void StopMonitoring()
        {
            if (!IsMonitoring) return;

            _cts.Cancel();
            IsMonitoring = false;
        }
    }
}

using Microsoft.Extensions.Configuration;
using PDFDataExtraction.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PDFDataExtraction
{
    public class MonitoringService : IMonitoringService
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly IConfiguration _configuration;
        private readonly Action<string> _notifyAction;

        public bool IsMonitoring { get; private set; } = false;

        public MonitoringService(IConfiguration configuration, Action<string> notifyAction)
        {
            _configuration = configuration;
            _notifyAction = notifyAction;
        }

        public void StartMonitoring()
        {
            if (IsMonitoring) return;

            IsMonitoring = true;
            
            // Reset the CancellationTokenSource for a new task
            _cts = new CancellationTokenSource(); 

            Task.Run(() => Program.MonitorPdfFolder(_cts.Token), _cts.Token);
            MonitorPdfFolder(_cts.Token);
        }

        private void MonitorPdfFolder(CancellationToken token)
        {
            _notifyAction("PDF folder is now being monitored.");
        }

        public void StopMonitoring()
        {
            if (!IsMonitoring) return;

            _cts.Cancel();
            IsMonitoring = false;
            _notifyAction("Monitoring stopped.");
        }
    }
}

using Microsoft.Extensions.Configuration;
using PDFDataExtraction.Core;
using System.Windows;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PDFDataExtraction
{
    public class MonitoringService : IMonitoringService
    {
        public event EventHandler<string> NotificationEvent;

        private CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly IConfiguration _configuration;

        public bool IsMonitoring { get; private set; } = false;

        public MonitoringService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public void StartMonitoring()
        {
            if (IsMonitoring) return;

            IsMonitoring = true;

            // Reset the CancellationTokenSource for a new task
            _cts = new CancellationTokenSource();

            Task.Run(() => Program.MonitorPdfFolder(_cts.Token), _cts.Token);
            MainWindow.ShowOverlay("PDF folder is now being monitored.");

        }


        public void StopMonitoring()
        {
            if (!IsMonitoring) return;

            _cts.Cancel();
            IsMonitoring = false;
            MainWindow.ShowOverlay("PDF folder is not being monitored any longer.");
        }
    }
}

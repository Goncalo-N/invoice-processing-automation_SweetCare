using System.Threading;
using PDFDataExtraction.Core;

namespace PDFDataExtraction
{
    public class MonitoringService : IMonitoringService
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();
        
        public bool IsMonitoring { get; private set; } = false;

        public void StartMonitoring()
        {
            if (IsMonitoring) return;

            IsMonitoring = true;
            string baseDirectory = Program.GetBaseDirectory();
            var folders = Program.GetFolderPaths(baseDirectory);
            _cts = new CancellationTokenSource(); // Reset the CancellationTokenSource for a new task
            Task.Run(() => Program.MonitorPdfFolder(folders.folderPath, folders.outputFolderPath, folders.validatedFolderPath, _cts.Token), _cts.Token);
        }

        public void StopMonitoring()
        {
            if (!IsMonitoring) return;

            _cts.Cancel();
            IsMonitoring = false;
        }
    }
}

namespace PDFDataExtraction
{
    public interface IMonitoringService
    {
        bool IsMonitoring { get; }
        void StartMonitoring();
        void StopMonitoring();
    }
}

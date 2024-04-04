using System.Windows;
using System.Windows.Media;

namespace PDFDataExtraction
{
    public partial class MainWindow : Window
    {
        private readonly IMonitoringService _monitoringService;

        public MainWindow()
        {
            InitializeComponent();
            _monitoringService = new MonitoringService(); // Ideally injected through DI
            ExitButton.Background = Brushes.Red;
        }

        private void StartPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_monitoringService.IsMonitoring)
            {
                _monitoringService.StartMonitoring();
                UpdateUIForMonitoringState();
            }
            else
            {
                _monitoringService.StopMonitoring();
                UpdateUIForMonitoringState();
            }
        }

        private void UpdateUIForMonitoringState()
        {
            if (_monitoringService.IsMonitoring)
            {
                StartPauseButton.Content = "Pause";
                StartPauseButton.Background = Brushes.Red;
                StatusIndicator.Background = Brushes.Green;
                StatusLabel.Content = "Running";
            }
            else
            {
                StartPauseButton.Content = "Start";
                StartPauseButton.Background = Brushes.Green;
                StatusIndicator.Background = Brushes.Red;
                StatusLabel.Content = "Paused";
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

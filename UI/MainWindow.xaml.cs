using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.Extensions.Configuration;

namespace PDFDataExtraction
{
    public partial class MainWindow : Window
    {
        private readonly IMonitoringService _monitoringService;

        public MainWindow()
        {
            
            InitializeComponent();

            // Manually build configuration
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Pass the configuration to MonitoringService
            _monitoringService = new MonitoringService(configuration);
            
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

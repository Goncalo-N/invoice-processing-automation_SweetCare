using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.Extensions.Configuration;

namespace PDFDataExtraction
{
    public partial class MainWindow : Window
    {
        private readonly IMonitoringService _monitoringService;
        private readonly IConfiguration _configuration;


        public MainWindow()
        {

            InitializeComponent();

            // Manually build configuration
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .Build();

            _configuration = configuration;
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


        public void InvalidFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var invalidFolder = _configuration["ApplicationPaths:InvalidFolder"] ?? throw new InvalidOperationException("ValidFolder Path not found in appsettings.json");

            // Ensure the directory exists before trying to open it
            if (!Directory.Exists(invalidFolder))
            {
                MessageBox.Show($"The folder '{invalidFolder}' does not exist.", "Folder Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Open the folder in File Explorer
            Process.Start("explorer.exe", invalidFolder);
        }
        public void ValidFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var validFolder = _configuration["ApplicationPaths:ValidFolder"] ?? throw new InvalidOperationException("ValidFolder Path not found in appsettings.json");

            // Ensure the directory exists before trying to open it
            if (!Directory.Exists(validFolder))
            {
                MessageBox.Show($"The folder '{validFolder}' does not exist.", "Folder Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Open the folder in File Explorer
            Process.Start("explorer.exe", validFolder);
        }

        public void LogsFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var logsFolder = _configuration["ApplicationPaths:LogsFolder"] ?? throw new InvalidOperationException("LogsFolder Path not found in appsettings.json");

            // Ensure the directory exists before trying to open it
            if (!Directory.Exists(logsFolder))
            {
                MessageBox.Show($"The folder '{logsFolder}' does not exist.", "Folder Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Open the folder in File Explorer
            Process.Start("explorer.exe", logsFolder);
        }
    }
}

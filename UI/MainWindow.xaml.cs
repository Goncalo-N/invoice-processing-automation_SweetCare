﻿using System.Diagnostics;
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
            _monitoringService = new MonitoringService(configuration);

            ExitButton.Background = System.Windows.Media.Brushes.Red;
        }

        public static void ShowOverlay(string message)
        {
            if (System.Windows.Application.Current != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    var overlay = new OverlayWindow(message);
                    overlay.Show();
                });
            }
            else
            {
                // Handle the case where no current application is available
                Console.WriteLine("Application context is not available.");
            }
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
                StartPauseButton.Background = System.Windows.Media.Brushes.Red;
                StatusIndicator.Background = System.Windows.Media.Brushes.Green;
                StatusLabel.Content = "Monitoring active";
            }
            else
            {
                StartPauseButton.Content = "Start";
                StartPauseButton.Background = System.Windows.Media.Brushes.Green;
                StatusIndicator.Background = System.Windows.Media.Brushes.Red;
                StatusLabel.Content = "Monitoring paused";
            }
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        public void InvalidFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var invalidFolder = _configuration["ApplicationPaths:InvalidFolder"] ?? throw new InvalidOperationException("Invalid Folder Path not found in appsettings.json");
            Process.Start("explorer.exe", invalidFolder);
        }

        public void ValidFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var validFolder = _configuration["ApplicationPaths:ValidFolder"] ?? throw new InvalidOperationException("Valid Folder Path not found in appsettings.json");
            Process.Start("explorer.exe", validFolder);
        }

        public void LogsFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var logsFolder = _configuration["ApplicationPaths:LogsFolder"] ?? throw new InvalidOperationException("Logs Folder Path not found in appsettings.json");
            Process.Start("explorer.exe", logsFolder);
        }
    }
}

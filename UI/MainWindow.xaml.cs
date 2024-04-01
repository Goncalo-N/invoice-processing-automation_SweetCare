using System;
using System.Reflection.Emit;
using System.Threading;
using System.Windows;
using Serilog;

namespace PDFDataExtraction
{
    public partial class MainWindow : Window
    {
        private bool _isMonitoring = false;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartPauseButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggling the monitoring status
            _isMonitoring = !_isMonitoring;

            if (_isMonitoring)
            {
                string baseDirectory = Program.GetBaseDirectory();
                var folders = Program.GetFolderPaths(baseDirectory);
                Console.WriteLine("Starting the task");
                _cts = new CancellationTokenSource(); // Reset the CancellationTokenSource for a new task
                Task.Run(() => Program.MonitorPdfFolder(folders.folderPath, folders.outputFolderPath, folders.validatedFolderPath, _cts.Token),_cts.Token);
                StartPauseButton.Content = "Pause";
                Status.Content = "Running"; // Update label content to "Running" when monitoring starts
            }
            else
            {
                // Pausing the monitoring
                Console.WriteLine("Pausing the task");
                _cts.Cancel();
                StartPauseButton.Content = "Resume";
                Status.Content = "Paused"; // Update label content to "Paused" when monitoring is paused
            }
        }

        //exit button
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the application
            Application.Current.Shutdown();
        }
    }
}

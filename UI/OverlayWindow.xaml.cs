using System.Windows;
using System.Windows.Threading;

namespace PDFDataExtraction
{
    public partial class OverlayWindow : Window
    {
        public string Message { get; set; }

        public OverlayWindow(string message)
        {
            InitializeComponent();
            DataContext = this;
            Message = message;

            PositionRelativeToMainWindow();
            StartCloseTimer();
        }

        private void PositionRelativeToMainWindow()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow;
            if (mainWindow != null)
            {
                // Set the position to the top left of the main window
                this.Left = mainWindow.Left;
                this.Top = mainWindow.Top+20;
            }
        }

        private void StartCloseTimer()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            timer.Tick += (sender, args) =>
            {
                timer.Stop();
                this.Close();
            };
            timer.Start();
        }
    }
}

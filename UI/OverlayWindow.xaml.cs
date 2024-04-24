using System.Windows;

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
        }

    }
}

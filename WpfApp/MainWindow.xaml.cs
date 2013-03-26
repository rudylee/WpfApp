using System.Net;
using System.Windows;

namespace WpfApp
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;

            var videoPage = new VideoPage();
            pageTransitionControl.ShowPage(videoPage);
        }
    }
}
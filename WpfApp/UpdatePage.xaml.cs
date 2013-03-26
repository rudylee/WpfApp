using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp.Core;
using WpfPageTransitions;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for UpdatePage.xaml
    /// </summary>
    public partial class UpdatePage : UserControl
    {
        public UpdatePage()
        {
            InitializeComponent();

            var appUpdate = new AppUpdate(DownloadProgress,GoToApp, ProgressBar);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var pageTransitionControl = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "pageTransitionControl") as PageTransition;
            if (pageTransitionControl == null) return;
            pageTransitionControl.TransitionType =
                (PageTransitionType)Enum.Parse(typeof(PageTransitionType), "SlideAndFade", true);
            pageTransitionControl.ShowPage(new VideoPage());
        }
    }
}

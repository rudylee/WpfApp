using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfPageTransitions;

namespace WpfApp
{
    public partial class FindToyota
    {
        readonly List<Button> _buttons;
        private int _start;

        public FindToyota()
        {
            InitializeComponent();

            _buttons = new List<Button> {Back };

            Loaded += delegate
            {
                var kinectControl = new KinectControl(KinectButton, LayoutRoot.ActualHeight, LayoutRoot.ActualWidth,
                                                   _buttons);
            };

            var imageSourceUri = String.Format(@"C:\Users\NICO\Desktop\aurion\aurion0.png");
            CarImage.Source = new BitmapImage(new Uri(imageSourceUri));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var pageTransitionControl = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "pageTransitionControl") as PageTransition;
            if (pageTransitionControl != null) pageTransitionControl.ShowPage(new HomePage());
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            _start += 1;
            var imageSourceUri = String.Format(@"C:\Users\NICO\Desktop\aurion\aurion" + _start +".png");
            CarImage.Source = new BitmapImage(new Uri(imageSourceUri));
        }
    }
}

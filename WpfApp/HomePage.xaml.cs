using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using WpfPageTransitions;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage 
    {
        private readonly PageTransition _pageTransitionControl;
        readonly List<Button> _buttons;

        public HomePage()
        {
            InitializeComponent();

            _pageTransitionControl = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "pageTransitionControl") as PageTransition;


            _buttons = new List<Button> { Welcome,Findtoyota,Dynamic,Gesture };

            Loaded += delegate
            {
                var kinectControl = new KinectControl(KinectButton, LayoutRoot.ActualHeight, LayoutRoot.ActualWidth,
                                                   _buttons);
            };

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _pageTransitionControl.ShowPage(new FindToyota());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _pageTransitionControl.ShowPage(new VideoPage());
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            _pageTransitionControl.ShowPage(new GestureMenu());
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            _pageTransitionControl.ShowPage(new DynamicMenu());
        }

    }
}

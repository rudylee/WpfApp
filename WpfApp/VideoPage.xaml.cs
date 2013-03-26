using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using WpfApp.Core;
using WpfPageTransitions;

namespace WpfApp
{
    public partial class VideoPage
    {
        private int _currentVideo;
        private readonly List<string> _videoList = new List<string>();
        List<Button> _buttons;

        public VideoPage()
        {
            InitializeComponent();

            var xmlHandler = new XmlHandler();
            _videoList = xmlHandler.GetVideos();

            _buttons = new List<Button> { Closevideo };

            Loaded += delegate
                {
                    var kinectControl = new KinectControl(KinectButton, LayoutRoot.ActualHeight,
                                                          LayoutRoot.ActualWidth,
                                                          _buttons);

                    VideoControl.Width = LayoutRoot.ActualWidth;
                    VideoControl.Height = LayoutRoot.ActualHeight;

                    _currentVideo = 0;
                    PlayVideo(_currentVideo);
                };

        }

        private void PlayVideo(int videoNumber)
        {
            VideoControl.Source = new Uri(_videoList[videoNumber]);
            VideoControl.Play();
        }

        private void VideoControl_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            var nextVideo = _currentVideo + 1;
            _currentVideo = nextVideo > _videoList.Count - 1 ? 0 : nextVideo;
            PlayVideo(_currentVideo);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var pageTransitionControl = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "pageTransitionControl") as PageTransition;
            if (pageTransitionControl == null) return;
            pageTransitionControl.TransitionType =
                (PageTransitionType)Enum.Parse(typeof(PageTransitionType), "SlideAndFade", true);
            pageTransitionControl.ShowPage(new HomePage());
        }

    }
}

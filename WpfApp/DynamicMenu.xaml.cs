using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace WpfApp
{
    public partial class DynamicMenu 
    {
        private XmlDocument _xmlDoc;
        private XmlNode _videos;
        private readonly List<string> _videoList = new List<string>();
        private readonly List<TextBlock> _menus = new List<TextBlock>();

        public DynamicMenu()
        {
            InitializeComponent();

            Loaded += delegate
                {
                    _xmlDoc = new XmlDocument();
                    _xmlDoc.Load("Content/content.xml");

                    _videos = _xmlDoc.SelectSingleNode("videos");

                    if (_videos == null) return;
                    for (var i = 0; i < _videos.ChildNodes.Count; i++)
                    {
                        var videoTitle = _videos.ChildNodes[i].ChildNodes[0].InnerText;
                        _videoList.Add(_videos.ChildNodes[i].ChildNodes[1].InnerText);

                        GenerateVideoList(i, videoTitle);
                    }
                    AddBackButton();
                    // ReSharper disable ObjectCreationAsStatement
                    new DynamicKinect(LayoutRoot.ActualHeight, LayoutRoot.ActualWidth, _menus, MenuStackPanel, DebugBox);
                    // ReSharper restore ObjectCreationAsStatement
                };
        }

        private void AddBackButton()
        {
            var mouseOverStyle = (Style)FindResource("MouseOverStyle");
            var backTextBlock = new TextBlock
            {
                Name = "Back",
                Text = "Back",
                FontSize = 60,
                Style = mouseOverStyle,
                TextAlignment = TextAlignment.Right
            };
            backTextBlock.MouseDown += MouseDownEvent;
            MenuStackPanel.Children.Add(backTextBlock);
            _menus.Add(backTextBlock);
        }

        private void GenerateVideoList(int i, string videoTitle)
        {
            var mouseOverStyle = (Style) FindResource("MouseOverStyle");
            var videoTextBlock = new TextBlock
                {
                    Name = "Video" + i,
                    Text = videoTitle,
                    FontSize = 60,
                    Style = mouseOverStyle,
                    TextAlignment = TextAlignment.Right
                };
            videoTextBlock.MouseDown += MouseDownEvent;
            MenuStackPanel.Children.Add(videoTextBlock);
            _menus.Add(videoTextBlock);
        }

        private new void MouseDownEvent(object sender, RoutedEventArgs e)
        {
            var sourceText = e.Source as TextBlock;
            //MessageBox.Show(sourceText.Text);
            if (sourceText != null && sourceText.Text != "Back")
            {
                PlayVideo(Convert.ToInt32(sourceText.Name.Substring(sourceText.Name.Length - 1)));
            }
        }

        private void PlayVideo(int videoNumber)
        {
            dvdPlayer.Source = new Uri(_videoList[videoNumber]);
            dvdPlayer.Play();
        }
    }
}

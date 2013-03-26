using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WpfPageTransitions;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for GestureMenu.xaml
    /// </summary>
    public partial class GestureMenu
    {
        private readonly List<TextBlock> _menus;
        private int _currentMenu;
        private double _startPoint;
        private Style _mouseOverStyle;

        public GestureMenu()
        {
            InitializeComponent();

            _menus = new List<TextBlock> {Menu1, Menu2, Menu3};

            Loaded += delegate
            {
                _mouseOverStyle = (Style)FindResource("MouseOverStyle");
                Menu1.Style = _mouseOverStyle;

                var kinectControl = new KinectDance(LayoutRoot.ActualHeight, LayoutRoot.ActualWidth,
                                                    _menus, _mouseOverStyle, MenuBorder, debugBox);
                _currentMenu = 1;
                _startPoint = 0;
            };

        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_KeyDownEvent(object sender, RoutedEventArgs e)
        {
            var sourceText = e.Source as TextBlock;
            //MessageBox.Show(sourceText.Name);

            var pageTransitionControl = LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "pageTransitionControl") as PageTransition;
            if (pageTransitionControl == null) return;
            pageTransitionControl.TransitionType =
                (PageTransitionType)Enum.Parse(typeof(PageTransitionType), "SlideAndFade", true);
            pageTransitionControl.ShowPage(new VideoPage());
        }

        private void ArrowUp_MouseEnter(object sender, MouseEventArgs mouseEventArgs)
        {
            _currentMenu = Math.Abs(_currentMenu - 1);

            var offset = VisualTreeHelper.GetOffset(MenuBorder);
            var top = offset.Y;

            var menuOffset = VisualTreeHelper.GetOffset(_menus[_currentMenu]);
            var topOffSet = menuOffset.Y;

            var animation = new DoubleAnimationUsingKeyFrames { Duration = new Duration(TimeSpan.FromSeconds(1)) };
            var easingFunction = new QuarticEase { EasingMode = EasingMode.EaseInOut };
            var startAnimation = new EasingDoubleKeyFrame(_startPoint, KeyTime.FromPercent(0));
            var endAnimation = new EasingDoubleKeyFrame(topOffSet - top, KeyTime.FromPercent(1.0), easingFunction);
            animation.KeyFrames.Add(startAnimation);
            animation.KeyFrames.Add(endAnimation);

            var trans = new TranslateTransform();
            MenuBorder.RenderTransform = trans;
            trans.BeginAnimation(TranslateTransform.YProperty, animation);

            _startPoint = topOffSet - top;
        }

        private void ArrowDown_MouseDown(object sender, MouseEventArgs e)
        {
            _currentMenu = Math.Abs(_currentMenu + 1);

            var offset = VisualTreeHelper.GetOffset(MenuBorder);
            var top = offset.Y;

            var menuOffset = VisualTreeHelper.GetOffset(_menus[_currentMenu]);
            var topOffSet = menuOffset.Y;

            var animation = new DoubleAnimationUsingKeyFrames { Duration = new Duration(TimeSpan.FromSeconds(1)) };
            var easingFunction = new QuarticEase { EasingMode = EasingMode.EaseInOut };
            var startAnimation = new EasingDoubleKeyFrame(_startPoint, KeyTime.FromPercent(0));
            var endAnimation = new EasingDoubleKeyFrame(topOffSet - top, KeyTime.FromPercent(1.0), easingFunction);
            animation.KeyFrames.Add(startAnimation);
            animation.KeyFrames.Add(endAnimation);

            var trans = new TranslateTransform();
            MenuBorder.RenderTransform = trans;
            trans.BeginAnimation(TranslateTransform.YProperty, animation);

            _startPoint = topOffSet - top;
        }
    }
}

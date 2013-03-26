using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Kinect;

namespace WpfApp
{
    class KinectDance
    {
        private readonly KinectSensor _kinect;
        private readonly double _layoutWidth;
        private readonly double _layoutHeight;
        private static TextBlock _selectedTextBlock;
        private static TextBox _debugBox;
        private static List<TextBlock> _menus;
        private static Style _mouseOverStyle;
        private static Border _menuBorder;
        private int _currentMenu;
        private double _startPoint = 0;
        private bool _animationRunning = false;

        private bool _swipeMenu = true;

        public KinectDance(double layoutHeight, double layoutWidth, List<TextBlock> menus, Style mouseOverStyle, Border menuBorder,TextBox debugBox = null)
        {
            _layoutHeight = layoutHeight;
            _layoutWidth = layoutWidth;
            _debugBox = debugBox;
            _menus = menus;
            _menuBorder = menuBorder;
            _mouseOverStyle = mouseOverStyle;

            _kinect = KinectSensor.KinectSensors.FirstOrDefault();

            if (_kinect == null) return;
            //_kinect.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
            _kinect.Start();

            _kinect.ColorStream.Enable();
            _kinect.SkeletonStream.Enable(new TransformSmoothParameters
                {
                    Smoothing = 0.7f,
                    Correction = 0.3f,
                    Prediction = 0.4f,
                    JitterRadius = 0.5f,
                    MaxDeviationRadius = 0.5f
                });

            _kinect.SkeletonFrameReady += kinect_SkeletonFrameReady;
        }
        private void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = null;

            using (var frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                }
            }

            if (skeletons == null) return;

            foreach (var skeleton in skeletons)
            {
                if (skeleton.TrackingState != SkeletonTrackingState.Tracked) continue;
                var rightHand = skeleton.Joints[JointType.HandRight];

                var rightHandScaled = rightHand.ScaleTo((int) (_layoutWidth/2), (int) _layoutHeight, 0.2f, 0.5f);
                var handX = rightHandScaled.Position.X;
                var handY = rightHandScaled.Position.Y;

                var rightWrist = skeleton.Joints[JointType.WristRight];
                var rightElbow = skeleton.Joints[JointType.ElbowRight];
                var head = skeleton.Joints[JointType.Head];
                var hip = skeleton.Joints[JointType.HipRight];

                if (rightElbow.Position.X < rightHand.Position.X && rightWrist.Position.X < rightHand.Position.X)
                {
                    _swipeMenu = false;
                    var currentSelected = _selectedTextBlock;
                    if (handY < 100)
                    {
                    }
                    else if (handY < 300 && handY > 100)
                    {
                        _menus[0].Style = _mouseOverStyle;
                        _menus[1].Style = null;
                        _menus[2].Style = null;
                        _selectedTextBlock = _menus[0];
                        _currentMenu = 0;

                        AnimateBorder();
                    }
                    else if (handY > 900)
                    {
                        _menus[2].Style = _mouseOverStyle;
                        _menus[0].Style = null;
                        _menus[1].Style = null;
                        _selectedTextBlock = _menus[2];
                        _currentMenu = 2;

                        AnimateBorder();
                    }
                    else if (handY < 750 && handY > 450)
                    {
                        _menus[1].Style = _mouseOverStyle;
                        _menus[0].Style = null;
                        _menus[2].Style = null;
                        _selectedTextBlock = _menus[1];
                        _currentMenu = 1;

                        AnimateBorder();
                    }

                    if (currentSelected != null)
                    {
                        var currentMouseEventArgs = new MouseEventArgs(Mouse.PrimaryDevice, 0)
                            {
                                RoutedEvent = Mouse.MouseLeaveEvent
                            };
                        currentSelected.RaiseEvent(currentMouseEventArgs);
                    }

                    var mouseEventArgs = new MouseEventArgs(Mouse.PrimaryDevice, 0) { RoutedEvent = Mouse.MouseEnterEvent };
                    if (_selectedTextBlock != null) _selectedTextBlock.RaiseEvent(mouseEventArgs);
                }
                if(rightHand.Position.X < head.Position.X && _swipeMenu == false && hip.Position.Y < rightHand.Position.Y)
                {
                    _swipeMenu = true;

                    if (_selectedTextBlock != null)
                    {
                        var mouseEventArgs = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = Mouse.MouseDownEvent };
                        _selectedTextBlock.RaiseEvent(mouseEventArgs);

                        _kinect.SkeletonFrameReady -= kinect_SkeletonFrameReady;
                    }
                }

                // debug textbox
                if (_debugBox != null)
                {
                    _debugBox.Text = handY.ToString();
                }
            }
        }

        private void AnimateBorder()
        {
            if (_animationRunning) return;

            _animationRunning = true;
            var offset = VisualTreeHelper.GetOffset(_menuBorder);
            var top = offset.Y;

            var menuOffset = VisualTreeHelper.GetOffset(_menus[_currentMenu]);
            var topOffSet = menuOffset.Y;

            var animation = new DoubleAnimationUsingKeyFrames {Duration = new Duration(TimeSpan.FromSeconds(0.5))};
            var easingFunction = new QuarticEase {EasingMode = EasingMode.EaseInOut};
            var startAnimation = new EasingDoubleKeyFrame(_startPoint, KeyTime.FromPercent(0));
            var endAnimation = new EasingDoubleKeyFrame(topOffSet - top, KeyTime.FromPercent(1.0), easingFunction);
            animation.KeyFrames.Add(startAnimation);
            animation.KeyFrames.Add(endAnimation);

            animation.Completed += delegate
            {
                _animationRunning = false;
            };

            var trans = new TranslateTransform();
            _menuBorder.RenderTransform = trans;
            trans.BeginAnimation(TranslateTransform.YProperty, animation);
  
            _startPoint = topOffSet - top;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Kinect;

namespace WpfApp
{
    class DynamicKinect
    {
        private readonly KinectSensor _kinect;
        private readonly double _layoutWidth;
        private readonly double _layoutHeight;
        private static TextBlock _selectedTextBlock;
        private static List<TextBlock> _menus;

        private bool _swipeMenu = true;
        private StackPanel _menuStackPanel;
        private readonly TextBlock _debugBox;
        private int _currentPanel;
        private DateTime? _nextAllowedClick;

        public DynamicKinect(double layoutHeight, double layoutWidth, List<TextBlock> menus, StackPanel menuStackPanel, TextBlock debugBox = null)
        {
            _layoutHeight = layoutHeight;
            _layoutWidth = layoutWidth;
            _menus = menus;
            _menuStackPanel = menuStackPanel;
            _debugBox = debugBox;


            _kinect = KinectSensor.KinectSensors.FirstOrDefault();

            if (_kinect == null) return;
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

                var rightHandScaled = rightHand.ScaleTo((int)(_layoutWidth / 2), (int)_layoutHeight, 0.2f, 0.5f);
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
                        if(_currentPanel > 0) _currentPanel -= 1;
                    }
                    else if (handY < 300 && handY > 100)
                    {
                        _selectedTextBlock = _menus[_currentPanel + 0];

                    }
                    else if (handY > 850 && handY < 980)
                    {
                        _selectedTextBlock = _menus[_currentPanel + 2];

                    }
                    else if (handY < 750 && handY > 450)
                    {
                        _selectedTextBlock = _menus[_currentPanel + 1];
                    }
                    else if (handY > 980)
                    {
                        if (_nextAllowedClick != null && DateTime.Now < _nextAllowedClick)
                        {
                            if (_currentPanel < _menus.Count - 3) _currentPanel += 1;
                        }
                        _nextAllowedClick = DateTime.Now + new TimeSpan(0, 0, 0, 2);
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

                    _debugBox.Text = _currentPanel.ToString() + '-' + handY.ToString() + '-' + _menus.Count.ToString();

                }
                if (rightHand.Position.X < head.Position.X && _swipeMenu == false && hip.Position.Y < rightHand.Position.Y)
                {
                    _swipeMenu = true;

                    if (_selectedTextBlock != null)
                    {
                        var mouseEventArgs = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = Mouse.MouseDownEvent };
                        _selectedTextBlock.RaiseEvent(mouseEventArgs);

                        //_kinect.SkeletonFrameReady -= kinect_SkeletonFrameReady;
                    }
                }
            }
        }

    }
}

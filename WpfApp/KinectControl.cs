using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Coding4Fun.Kinect.Wpf.Controls;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Kinect;
using Microsoft.Samples.Kinect.SwipeGestureRecognizer;

namespace WpfApp
{
    class KinectControl
    {
        private readonly KinectSensor _kinect;
        private readonly HoverButton _kinectButton;
        private readonly double _layoutWidth;
        private readonly double _layoutHeight;
        private readonly List<Button> _buttons;
        static Button _selected;
        private static TextBox _debugBox;

        private readonly Recognizer _activeRecognizer;

        public KinectControl(HoverButton kinectButton, double layoutHeight, double layoutWidth, List<Button> buttons, TextBox debugBox = null)
        {
            _kinectButton = kinectButton;
            _layoutHeight = layoutHeight;
            _layoutWidth = layoutWidth;
            _buttons = buttons;
            _debugBox = debugBox;

            _kinect = KinectSensor.KinectSensors.FirstOrDefault();

            if (_kinect != null)
            {
                _kinect.Start();

                _kinect.ColorStream.Enable();
                _kinect.SkeletonStream.Enable(new TransformSmoothParameters
                    {
                        Smoothing = 0.7f,
                        Correction = 0.1f,
                        Prediction = 0.1f,
                        JitterRadius = 0.05f,
                        MaxDeviationRadius = 0.05f
                    });

                _kinect.SkeletonFrameReady += kinect_SkeletonFrameReady;
            }

            _activeRecognizer = CreateRecognizer();
            _kinectButton.Click += KinectButton_Click;
        }

        private static Skeleton GetPrimarySkeleton(IEnumerable<Skeleton> skeletons)
        {
            Skeleton primarySkeleton = null;
            foreach (var skeleton in skeletons.Where(skeleton => skeleton.TrackingState == SkeletonTrackingState.Tracked))
            {
                if (primarySkeleton == null)
                    primarySkeleton = skeleton;
                else if (primarySkeleton.Position.Z > skeleton.Position.Z)
                    primarySkeleton = skeleton;
            }
            return primarySkeleton;
        }

        private Recognizer CreateRecognizer()
        {
            // Instantiate a recognizer.
            var recognizer = new Recognizer();

            // Wire-up swipe right to manually advance picture.
            //recognizer.SwipeRightDetected += (s, e) =>
            //{
            //    _selected = _buttons.First();
            //    _selected.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, _selected));
            //};

            // Wire-up swipe right to manually advance picture.
            //recognizer.SwipeLeftDetected += (s, e) =>
            //{
            //    _selected = _buttons[1];
            //    _selected.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, _selected));
            //};

            return recognizer;
        }

        private void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = null;

            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                    _activeRecognizer.Recognize(sender, frame, skeletons);
                }
            }

            if (skeletons == null) return;

            foreach (var skeleton in skeletons)
            {
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    var rightHand = skeleton.Joints[JointType.HandRight];
                    _kinectButton.Visibility = Visibility.Visible;

                    //var point = _kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(rightHand.Position, DepthImageFormat.Resolution640x480Fps30);
                    //var handX = (int)((point.X * _layoutWidth / _kinect.DepthStream.FrameWidth) -
                    //    (_kinectButton.ActualWidth / 2.0));
                    //var handY = (int)((point.Y * _layoutHeight / _kinect.DepthStream.FrameHeight) -
                    //    (_kinectButton.ActualHeight / 2.0));

                    var rightHandScaled = rightHand.ScaleTo((int) _layoutWidth - (int) _kinectButton.ActualWidth, (int) _layoutHeight - (int) _kinectButton.ActualHeight, 0.2f, 0.4f);
                    var handX = rightHandScaled.Position.X;
                    var handY = rightHandScaled.Position.Y;

                    Canvas.SetLeft(_kinectButton, handX);
                    Canvas.SetTop(_kinectButton, handY);

                    if (isHandOver(_kinectButton, _buttons)) _kinectButton.Hovering();
                    else _kinectButton.Release();

                    // debug textbox
                    if (_debugBox != null)
                    {
                        _debugBox.Text = handX.ToString();
                    }
                }
            }
        }

        private bool isHandOver(FrameworkElement hand, IEnumerable<Button> list)
        {
            var handTopLeft = new Point(Canvas.GetLeft(hand), Canvas.GetTop(hand));
            var handX = handTopLeft.X + hand.ActualWidth / 2;
            var handY = handTopLeft.Y + hand.ActualHeight / 2;

            foreach (Button target in list)
            {
                if (target != null)
                {
                    var targetTopLeft = new Point(Canvas.GetLeft(target), Canvas.GetTop(target));
                    if (handX > targetTopLeft.X &&
                        handX < targetTopLeft.X + target.Width &&
                        handY > targetTopLeft.Y &&
                        handY < targetTopLeft.Y + target.Height)
                    {
                        _selected = target;
                        return true;
                    }
                }
            }
            return false;
        }

        private void KinectButton_Click(object sender, RoutedEventArgs e)
        {
            _selected.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, _selected));
            _kinect.SkeletonFrameReady -= kinect_SkeletonFrameReady;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Button = System.Windows.Controls.Button;
using ProgressBar = System.Windows.Controls.ProgressBar;

namespace WpfApp.Core
{
    public class AppUpdate
    {
        private readonly TextBlock _progressTextBlock;
        private readonly Button _goToApp;
        private readonly ProgressBar _progressBar;
        private readonly WebClient _webClient = new WebClient();
        private readonly string _videoFolder;
        private List<string> _videosToDownload = new List<string>();
        private int _totalVideosToDownload;

        public AppUpdate(TextBlock progressTextBlock, Button goToApp, ProgressBar progressBar)
        {
            _progressTextBlock = progressTextBlock;
            _goToApp = goToApp;
            _progressBar = progressBar;
            var xmlUrl = new Uri(ConfigurationManager.AppSettings.Get("xmlURL"));
            var xmlFile = ConfigurationManager.AppSettings.Get("xmlFile");
            _videoFolder = ConfigurationManager.AppSettings.Get("videoFolder");

            _progressTextBlock.Text = "Synchronizing with main server...";
            _webClient.DownloadFileAsync(xmlUrl, xmlFile);
            _webClient.DownloadFileCompleted += XmlDownloadCompleted;
        }

        private void XmlDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            _webClient.DownloadFileCompleted -= XmlDownloadCompleted;
            _videosToDownload = GetVideosListToDownload();
            _totalVideosToDownload = _videosToDownload.Count;
            
            if (_videosToDownload.Count > 0)
            {
                var remoteVideoUrl = ConfigurationManager.AppSettings.Get("remoteVideoURL");
                var videoUri = new Uri(remoteVideoUrl + _videosToDownload[0]);

                _webClient.DownloadFileCompleted += VideoDownloadCompleted;
                _webClient.DownloadProgressChanged += VideoProgressChanged;
                _webClient.DownloadFileAsync(videoUri, _videoFolder + _videosToDownload[0]);
            }
            else
            {
                _goToApp.Visibility = Visibility.Visible;
            }
        }

        private void VideoProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _progressTextBlock.Text = String.Format("{0} Downloaded {1} of {2} bytes. {3} % Complete...",
                                                    (string)e.UserState, e.BytesReceived, e.TotalBytesToReceive,
                                                    e.ProgressPercentage);
            _progressBar.Value = e.ProgressPercentage;
        }

        private void VideoDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            _videosToDownload.RemoveAt(0);
            if (_videosToDownload.Count > 0)
            {
                var remoteVideoUrl = ConfigurationManager.AppSettings.Get("remoteVideoURL");
                var videoUri = new Uri(remoteVideoUrl + _videosToDownload[0]);
                _webClient.DownloadFileAsync(videoUri, _videoFolder + _videosToDownload[0]);
            }

            if (_videosToDownload.Count == 0)
            {
                _goToApp.Visibility = Visibility.Visible;
            }
        }

        private List<string> GetVideosListToDownload()
        {
            var xmlHandler = new XmlHandler();
            var videosList = xmlHandler.GetVideos();

            return videosList.Where(video => !CheckVideoExists(video)).ToList();
        }

        private bool CheckVideoExists(string fileName)
        {
            return File.Exists(_videoFolder + fileName);
        }

    }
}

using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace WpfApp.Core
{
    public class XmlHandler
    {
        private readonly XmlDocument _xmlDocument;

        public XmlHandler()
        {
            var xmlFile = ConfigurationManager.AppSettings.Get("xmlFile");

            _xmlDocument = new XmlDocument();
            _xmlDocument.Load(xmlFile);
        }

        public List<string> GetVideos()
        {
            var videos = _xmlDocument.SelectSingleNode("videos").SelectNodes("video");
            var videosList = new List<string>();

            if (videos == null) return null;
            foreach (XmlNode video in videos)
            {
                videosList.Add(video.SelectSingleNode("filename").InnerText);
            }
            return videosList;
        }
    }
}
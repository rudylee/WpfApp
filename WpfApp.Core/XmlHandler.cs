using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
            var selectSingleNode = _xmlDocument.SelectSingleNode("videos");
            if (selectSingleNode != null)
            {
                var videos = selectSingleNode.SelectNodes("video");
                return videos == null ? null : (from XmlNode video in videos let singleNode = video.SelectSingleNode("filename") where singleNode != null select singleNode.InnerText).ToList();
            }
            return null;
        }
    }
}
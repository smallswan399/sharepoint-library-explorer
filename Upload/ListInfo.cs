using System.Xml;

namespace Upload
{
    public class ListInfo
    {
        public string RootFolder;
        public string ListName;
        public string Version;
        public string WebUrl;
        public ListInfo(XmlNode listResponse)
        {
            RootFolder = listResponse.Attributes["RootFolder"].Value + "/";
            ListName = listResponse.Attributes["ID"].Value;
            Version = listResponse.Attributes["Version"].Value;
        }
        public bool IsMatch(string url)
        {
            try
            {
                url += "/";
                return url.Substring(0, RootFolder.Length) == RootFolder;
            }
            catch { }
            return false;
        }
    }
}
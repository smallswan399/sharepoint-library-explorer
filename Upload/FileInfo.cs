using System;
using System.Collections.Generic;

namespace Upload
{
    public class FileInfo
    {
        public string URL;
        public byte[] Bytes;
        public Dictionary<string, object> Properties;
        public ListInfo ListInfo;
        public bool EnsureFolders = true;
        private Uri uri;
        public bool HasProperties
        {
            get { return Properties != null && Properties.Count > 0; }
        }
        public string RelativeFilePath
        {
            get { return URL.Substring(URL.IndexOf(ListInfo.RootFolder) + 1); }
        }
        public Uri URI
        {
            get { return uri ?? (uri = new Uri(URL)); }
        }
        public string LookupName
        {
            get
            {
                if (ListInfo != null && !string.IsNullOrEmpty(ListInfo.ListName))
                    return ListInfo.ListName;
                return URI.LocalPath;
            }
        }
        public FileInfo(string url, byte[] bytes, Dictionary<string, object> properties)
        {
            URL = url.Replace("%20", " ");
            this.Bytes = bytes;
            Properties = properties;
        }
    }
}
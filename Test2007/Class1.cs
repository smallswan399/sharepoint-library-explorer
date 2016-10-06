using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Description;
using System.Xml;
using Main.ListServiceReference;

public class DocLibHelper
{
    Main.ListServiceReference.ListsSoapClient m_listService;
    ClientCredentials m_credentials;
    ListInfoCollection m_lists;

    public DocLibHelper()
    {
        m_credentials = CredentialCache.DefaultCredentials;
        m_listService = new ListsSoapClient();
        m_listService.ClientCredentials = m_credentials;
        m_lists = new ListInfoCollection(m_listService);
    }

    public class ListInfo
    {
        public string m_rootFolder;
        public string m_listName;
        public string m_version;
        public string m_webUrl;
        public ListInfo(XmlNode listResponse)
        {
            m_rootFolder = listResponse.Attributes["RootFolder"].Value + "/";
            m_listName = listResponse.Attributes["ID"].Value;
            m_version = listResponse.Attributes["Version"].Value;
        }
        public bool IsMatch(string url)
        {
            try
            {
                url += "/";
                return url.Substring(0, m_rootFolder.Length) == m_rootFolder;
            }
            catch { }
            return false;
        }
    }

    public class ListInfoCollection : IEnumerable<ListInfo>
    {
        ListsService.Lists m_listService;
        Dictionary<string, ListInfo> m_lists = new Dictionary<string, ListInfo>();
        public ListInfoCollection(ListsService.Lists listService)
        {
            m_listService = listService;
        }
        public IEnumerator<ListInfo> GetEnumerator()
        {
            return m_lists.Values.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        public ListInfo Find(FileInfo fileInfo)
        {
            if (m_lists.ContainsKey(fileInfo.LookupName))
                return m_lists[fileInfo.LookupName];
            foreach (ListInfo li in m_lists.Values)
                if (li.IsMatch(fileInfo.LookupName)) return li;
            string webUrl = fileInfo.m_URL;
            if (fileInfo.m_listInfo != null && !string.IsNullOrEmpty(fileInfo.m_listInfo.m_listName))
            {
                ListInfo listInfo = new ListInfo(CallService(ref webUrl, delegate { return m_listService.GetList(fileInfo.LookupName); }));
                listInfo.m_webUrl = webUrl;
                return listInfo;
            }
            else
            {
                XmlNode lists = CallService(ref webUrl, delegate { return m_listService.GetListCollection(); });
                if (lists == null) throw new Exception("Could not find web.");
                //Find list by RootFolder (which doesn't seem to be populated in GetListCollection response so must iterate GetList response)
                foreach (XmlNode list in lists.ChildNodes)
                {
                    ListInfo listInfo = new ListInfo(m_listService.GetList(list.Attributes["Name"].Value));
                    listInfo.m_webUrl = webUrl;
                    m_lists.Add(listInfo.m_listName, listInfo);
                    if (listInfo.IsMatch(fileInfo.LookupName))
                        return listInfo;
                }
            }
            throw new Exception("Could not find list.");
        }
        private delegate XmlNode ServiceOperation();
        private XmlNode CallService(ref string webURL, ServiceOperation serviceOperation)
        {
            try
            {
                webURL = webURL.Substring(0, webURL.LastIndexOf("/"));
                try
                {
                    m_listService.Url = webURL + "/_vti_bin/Lists.asmx";
                    return serviceOperation();
                }
                catch
                {
                    return CallService(ref webURL, serviceOperation);
                }
            }
            catch
            {
                webURL = null;
                return null;
            }
        }
    }

    public class FileInfo
    {
        public string m_URL;
        public byte[] m_bytes;
        public Dictionary<string, object> m_properties;
        public ListInfo m_listInfo;
        public bool m_ensureFolders = true;
        private Uri m_uri;
        public bool HasProperties
        {
            get { return m_properties != null && m_properties.Count > 0; }
        }
        public string RelativeFilePath
        {
            get { return m_URL.Substring(m_URL.IndexOf(m_listInfo.m_rootFolder) + 1); }
        }
        public Uri URI
        {
            get
            {
                if (m_uri == null) m_uri = new Uri(m_URL);
                return m_uri;
            }
        }
        public string LookupName
        {
            get
            {
                if (m_listInfo != null && !string.IsNullOrEmpty(m_listInfo.m_listName))
                    return m_listInfo.m_listName;
                return URI.LocalPath;
            }
        }
        public FileInfo(string url, byte[] bytes, Dictionary<string, object> properties)
        {
            m_URL = url.Replace("%20", " ");
            m_bytes = bytes;
            m_properties = properties;
        }
    }

    public bool Upload(string destinationUrl, byte[] bytes, Dictionary<string, object> properties)
    {
        return Upload(new FileInfo(destinationUrl, bytes, properties));
    }

    public bool Upload(FileInfo fileInfo)
    {
        if (fileInfo.HasProperties)
            fileInfo.m_listInfo = m_lists.Find(fileInfo);
        bool result = TryToUpload(fileInfo);
        if (!result && fileInfo.m_ensureFolders)
        {
            string root = fileInfo.URI.AbsoluteUri.Replace(fileInfo.URI.AbsolutePath, "");
            for (int i = 0; i < fileInfo.URI.Segments.Length - 1; i++)
            {
                root += fileInfo.URI.Segments[i];
                if (i > 1) CreateFolder(root);
            }
            result = TryToUpload(fileInfo);
        }
        return result;
    }

    private bool TryToUpload(FileInfo fileInfo)
    {
        try
        {
            WebRequest request = WebRequest.Create(fileInfo.m_URL);
            request.Credentials = m_credentials;
            request.Method = "PUT";
            byte[] buffer = new byte[1024];
            using (Stream stream = request.GetRequestStream())
            using (MemoryStream ms = new MemoryStream(fileInfo.m_bytes))
                for (int i = ms.Read(buffer, 0, buffer.Length); i > 0; i = ms.Read(buffer, 0, buffer.Length))
                    stream.Write(buffer, 0, i);
            WebResponse response = request.GetResponse();
            response.Close();
            if (fileInfo.HasProperties)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<Method ID='1' Cmd='Update'><Field Name='ID'/>");
                sb.AppendFormat("<Field Name='FileRef'>{0}</Field>", fileInfo.m_URL);
                foreach (KeyValuePair<string, object> property in fileInfo.m_properties)
                    sb.AppendFormat("<Field Name='{0}'>{1}</Field>", property.Key, property.Value);
                sb.Append("</Method>");
                System.Xml.XmlElement updates = (new System.Xml.XmlDocument()).CreateElement("Batch");
                updates.SetAttribute("OnError", "Continue");
                updates.SetAttribute("ListVersion", fileInfo.m_listInfo.m_version);
                updates.SetAttribute("PreCalc", "TRUE");
                updates.InnerXml = sb.ToString();
                m_listService.Url = fileInfo.m_listInfo.m_webUrl + "/_vti_bin/Lists.asmx";
                XmlNode updatesResponse = m_listService.UpdateListItems(fileInfo.m_listInfo.m_listName, updates);
                if (updatesResponse.FirstChild.FirstChild.InnerText != "0x00000000")
                    throw new Exception("Could not update properties.");
            }
            return true;
        }
        catch (WebException)
        {
            return false;
        }
    }

    private bool CreateFolder(string folderURL)
    {
        try
        {
            WebRequest request = WebRequest.Create(folderURL);
            request.Credentials = m_credentials;
            request.Method = "MKCOL";
            WebResponse response = request.GetResponse();
            response.Close();
            return true;
        }
        catch (WebException)
        {
            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Upload.ListsService;

namespace Upload
{
    public class DocLibHelper
    {
        readonly Lists listService;
        readonly ICredentials credentials;
        readonly ListInfoCollection lists;

        public DocLibHelper()
        {
            credentials = new NetworkCredential("sp2007admin", "@SharePoint2007!Admin");
            listService = new Lists {Credentials = credentials};
            lists = new ListInfoCollection(listService);
        }

        

        public bool Upload(string destinationUrl, byte[] bytes, Dictionary<string, object> properties)
        {
            return Upload(new FileInfo(destinationUrl, bytes, properties));
        }

        public bool Upload(FileInfo fileInfo)
        {
            if (fileInfo.HasProperties)
                fileInfo.ListInfo = lists.Find(fileInfo);
            bool result = TryToUpload(fileInfo);
            if (!result && fileInfo.EnsureFolders)
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
                WebRequest request = WebRequest.Create(fileInfo.URL);
                request.Credentials = credentials;
                request.Method = "PUT";
                request.Headers.Add("Overwrite", "t");
                byte[] buffer = new byte[1024];
                using (Stream stream = request.GetRequestStream())
                using (MemoryStream ms = new MemoryStream(fileInfo.Bytes))
                    for (int i = ms.Read(buffer, 0, buffer.Length); i > 0; i = ms.Read(buffer, 0, buffer.Length))
                        stream.Write(buffer, 0, i);
                WebResponse response = request.GetResponse();
                response.Close();
                if (fileInfo.HasProperties)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<Method ID='1' Cmd='Update'><Field Name='ID'/>");
                    sb.AppendFormat("<Field Name='FileRef'>{0}</Field>", fileInfo.URL);
                    foreach (KeyValuePair<string, object> property in fileInfo.Properties)
                        sb.AppendFormat("<Field Name='{0}'>{1}</Field>", property.Key, property.Value);
                    sb.Append("</Method>");
                    System.Xml.XmlElement updates = (new System.Xml.XmlDocument()).CreateElement("Batch");
                    updates.SetAttribute("OnError", "Continue");
                    updates.SetAttribute("ListVersion", fileInfo.ListInfo.Version);
                    updates.SetAttribute("PreCalc", "TRUE");
                    updates.InnerXml = sb.ToString();
                    listService.Url = fileInfo.ListInfo.WebUrl + "/_vti_bin/Lists.asmx";
                    XmlNode updatesResponse = listService.UpdateListItems(fileInfo.ListInfo.ListName, updates);
                    if (updatesResponse.FirstChild.FirstChild.InnerText != "0x00000000")
                        throw new Exception("Could not update properties.");
                }
                return true;
            }
            catch (WebException ex)
            {
                return false;
            }
        }

        private bool CreateFolder(string folderURL)
        {
            try
            {
                WebRequest request = WebRequest.Create(folderURL);
                request.Credentials = credentials;
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
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Upload
{
    public class ListInfoCollection : IEnumerable<ListInfo>
    {
        readonly ListsService.Lists listService;
        Dictionary<string, ListInfo> m_lists = new Dictionary<string, ListInfo>();
        public ListInfoCollection(ListsService.Lists listService)
        {
            this.listService = listService;
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
            string webUrl = fileInfo.URL.Replace(fileInfo.LookupName, "");
            if (fileInfo.ListInfo != null && !string.IsNullOrEmpty(fileInfo.ListInfo.ListName))
            {
                ListInfo listInfo = new ListInfo(CallService(ref webUrl, () => listService.GetList(fileInfo.LookupName)));
                listInfo.WebUrl = webUrl;
                return listInfo;
            }
            else
            {
                XmlNode lists = CallService(ref webUrl, () => listService.GetListCollection());
                if (lists == null) throw new Exception("Could not find web.");
                //Find list by RootFolder (which doesn't seem to be populated in GetListCollection response so must iterate GetList response)
                foreach (XmlNode list in lists.ChildNodes)
                {
                    ListInfo listInfo = new ListInfo(listService.GetList(list.Attributes["Name"].Value));
                    listInfo.WebUrl = webUrl;
                    m_lists.Add(listInfo.ListName, listInfo);
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
                // webURL = webURL.Substring(0, webURL.LastIndexOf("/"));
                try
                {
                    listService.Url = webURL + "/_vti_bin/Lists.asmx";
                    return serviceOperation();
                }
                catch(Exception ex)
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
}
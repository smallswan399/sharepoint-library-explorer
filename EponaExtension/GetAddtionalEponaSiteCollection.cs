using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using Extensions;
using Microsoft.SharePoint.Client;
using Site = Entities.Site;

namespace EponaExtension
{
    [Export(typeof(IAdditionalSPSite))]
    public class GetAddtionalEponaSiteCollection : IAdditionalSPSite
    {
        public IEnumerable<Site> GetAddtionalSites(Site baseSite)
        {
            var result = new List<Site>();
            if (baseSite?.Credential == null)
            {
                throw new ArgumentNullException(nameof(baseSite));
            }
            if (string.IsNullOrWhiteSpace(baseSite.Url) ||
                string.IsNullOrWhiteSpace(baseSite.Credential.Username))
            {
                throw new ArgumentException();
            }

            var url = baseSite.Url.TrimEnd('/');

            if (url != "http://dmsflcourse001.epona.com" && url != "https://dmsflcourse001.epona.com") 
                return result;

            // Get List items in 
            var context = new ClientContext(baseSite.Url)
            {
                Credentials = new NetworkCredential(baseSite.Credential.Username, baseSite.Credential.ToClearTextCredential().Password)
            };
            var listItems = context.Web.Lists.GetByTitle("Matters").GetItems(CamlQuery.CreateAllItemsQuery());
            context.Load(listItems);
            context.ExecuteQuery();

            result = listItems.Select(s => new Site()
            {
                RootUrl = baseSite.Url,
                Url = $"{baseSite.Url.TrimEnd('/')}/{s.FieldValues["SiteURL"].ToString().TrimStart('/')}",
                Enable = true,
                SharePointServerVersion = baseSite.SharePointServerVersion,
                Credential = baseSite.Credential,
                RequireAuthentication = baseSite.RequireAuthentication,

            }).ToList();

            return result;
        }
    }
}

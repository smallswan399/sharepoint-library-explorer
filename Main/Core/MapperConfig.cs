using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Libs;
using Entities;
using Main.Libs;
using Main.Services.Domains;
using Main.ViewModels;

namespace Main.Core
{
    public static class MapperConfig
    {
        public static SiteDetails ToSiteDetails(this Site site)
        {
            return new SiteDetails()
            {
                Credential = site.Credential.ToClearTextCredential(),
                Description = site.Description,
                Enable = site.Enable,
                IncludeSubSites = site.IncludeSubSites,
                Id = site.Id,
                Name = site.Name,
                RequireAuthentication = site.RequireAuthentication,
                SharePointServerVersion = site.SharePointServerVersion,
                Url = site.Url
            };
        }

        public static IEnumerable<SiteDetails> ToSiteDetails(IEnumerable<Site> sites)
        {
            return sites.Select(s => s.ToSiteDetails());
        }

        public static SiteListItem ToSiteListItem(this Site site)
        {
            return new SiteListItem()
            {
                CredentialUsername = site.Credential.Username,
                Description = site.Description,
                Enable = site.Enable ? "Y" : "N",
                Id = site.Id,
                Name = site.Name,
                Url = site.Url
            };
        }

        public static IEnumerable<SiteListItem> ToSiteListItem(this IEnumerable<Site> sites)
        {
            return sites.Select(s => s.ToSiteListItem());
        }


        public static Site ToSite(this SiteDetails siteDetails, Site site = null)
        {
            if (site == null)
            {
                return new Site()
                {
                    Credential = siteDetails.Credential.ToCredential(),
                    Description = siteDetails.Description,
                    Enable = siteDetails.Enable,
                    IncludeSubSites = siteDetails.IncludeSubSites,
                    Name = siteDetails.Name,
                    RequireAuthentication = siteDetails.RequireAuthentication,
                    SharePointServerVersion = siteDetails.SharePointServerVersion,
                    Url = siteDetails.Url,
                    Id = siteDetails.Id
                };
            }
            site.Credential = siteDetails.Credential.ToCredential();
            site.Description = siteDetails.Description;
            site.Enable = siteDetails.Enable;
            site.IncludeSubSites = siteDetails.IncludeSubSites;
            site.Name = siteDetails.Name;
            site.RequireAuthentication = siteDetails.RequireAuthentication;
            site.SharePointServerVersion = siteDetails.SharePointServerVersion;
            site.Url = siteDetails.Url;
            site.Id = siteDetails.Id;

            return site;
        }

        

        
    }
}

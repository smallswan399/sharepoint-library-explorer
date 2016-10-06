using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using Entities;
using Main.CopyServiceReference;
using Main.ListServiceReference;
using Main.Services.Domains;
using Main.SiteDataServiceReference;
using Main.SitesServiceReference;
using Main.UserProfileServiceReference;
using Main.VersionServiceReference;
using Main.WebServiceReference;
using Services;

namespace Main.Services
{
    public static class SoapClient2007Repo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper">Key to get exist ListsSoapClient instance</param>
        /// <param name="credential">Credential used to create new ListsSoapClient instance if need</param>
        /// <returns></returns>
        public static ListsSoapClient GetListsSoapClient(SiteSoapClientMapper mapper, ClearTextCredential credential)
        {
            var newListsSoapClient = new ListsSoapClient();
            newListsSoapClient.Endpoint.Address = new EndpointAddress(mapper.Url + "/_vti_bin/Lists.asmx");
            newListsSoapClient.ClientCredentials.Windows.ClientCredential = new NetworkCredential(credential.Username, credential.Password, credential.DomainName);
            newListsSoapClient.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Delegation;
            return newListsSoapClient;
        }

        public static WebsSoapClient GetWebsSoapClient(SiteSoapClientMapper mapper, ClearTextCredential credential)
        {
            var newWebsSoapClient = new WebsSoapClient();
            newWebsSoapClient.Endpoint.Address = new EndpointAddress(mapper.Url + "/_vti_bin/webs.asmx");
            newWebsSoapClient.ClientCredentials.Windows.ClientCredential = new NetworkCredential(credential.Username, credential.Password, credential.DomainName);
            newWebsSoapClient.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Delegation;
            return newWebsSoapClient;
        }

        public static CopySoapClient GetCopySoapClient(SiteSoapClientMapper mapper, ClearTextCredential credential)
        {
            var newCopySoapClient = new CopySoapClient();
            newCopySoapClient.Endpoint.Address = new EndpointAddress(mapper.Url + "/_vti_bin/copy.asmx");
            newCopySoapClient.ClientCredentials.Windows.ClientCredential = new NetworkCredential(credential.Username, credential.Password, credential.DomainName);
            newCopySoapClient.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Delegation;
            return newCopySoapClient;
        }

        public static VersionsSoapClient GetVersionsSoapClient(SiteSoapClientMapper mapper,
            ClearTextCredential credential)
        {
            var newVersionsSoapClient = new VersionsSoapClient();
            newVersionsSoapClient.Endpoint.Address = new EndpointAddress(mapper.Url + "/_vti_bin/Versions.asmx");
            newVersionsSoapClient.ClientCredentials.Windows.ClientCredential = new NetworkCredential(credential.Username, credential.Password, credential.DomainName);
            newVersionsSoapClient.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Delegation;
            return newVersionsSoapClient;
        }

        public static UserProfileServiceSoapClient GetUserProfileServiceSoapClient(SiteSoapClientMapper mapper,
            ClearTextCredential credential)
        {
            var userProfileServiceSoapClient = new UserProfileServiceSoapClient();
            userProfileServiceSoapClient.Endpoint.Address = new EndpointAddress(mapper.Url + "/_vti_bin/userprofileservice.asmx");
            userProfileServiceSoapClient.ClientCredentials.Windows.ClientCredential = new NetworkCredential(credential.Username, credential.Password, credential.DomainName);
            userProfileServiceSoapClient.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Delegation;
            return userProfileServiceSoapClient;

//            win-qmr53o3594d\nguyendangdung
        }

        public static UserProfileService.UserProfileService GetUserProfileService(SiteSoapClientMapper mapper,
            ClearTextCredential credential)
        {
            return new UserProfileService.UserProfileService()
            {
                Url = mapper.Url + "/_vti_bin/userprofileservice.asmx",
                Credentials = new NetworkCredential(credential.Username, credential.Password, credential.DomainName)
            };
        }

        public static SitesSoapClient GetSitesSoapClient(SiteSoapClientMapper mapper,
            ClearTextCredential credential)
        {
            var newSitesSoapClient = new SitesSoapClient();
            newSitesSoapClient.Endpoint.Address = new EndpointAddress(mapper.Url + "/_vti_bin/sites.asmx");
            newSitesSoapClient.ClientCredentials.Windows.ClientCredential = new NetworkCredential(credential.Username, credential.Password, credential.DomainName);
            newSitesSoapClient.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Delegation;
            return newSitesSoapClient;
        }

        public static SiteDataSoapClient GetSiteDataSoapClient(SiteSoapClientMapper mapper,
            ClearTextCredential credential)
        {
            var newSiteDataSoapClient = new SiteDataSoapClient();
            newSiteDataSoapClient.Endpoint.Address = new EndpointAddress(mapper.Url + "/_vti_bin/SiteData.asmx");
            newSiteDataSoapClient.ClientCredentials.Windows.ClientCredential = new NetworkCredential(credential.Username, credential.Password, credential.DomainName);
            newSiteDataSoapClient.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Delegation;
            return newSiteDataSoapClient;
        }
    }
}

using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using Entities;
using Main.Copy2010ServiceReference;
using Main.List2010ServiceReference;
using Main.Services.Domains;
using Main.Version2010ServiceReference;
using Main.Web2010ServiceReference;

namespace Services
{
    public static class SoapClient2010Repo
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
            if (newListsSoapClient.ClientCredentials.Windows.ClientCredential == null)
            {
                newListsSoapClient.ClientCredentials.Windows.ClientCredential = new NetworkCredential(credential.Username, credential.Password, credential.DomainName);
            }
            else
            {
                newListsSoapClient.ClientCredentials.Windows.ClientCredential.UserName = credential.Username;
                newListsSoapClient.ClientCredentials.Windows.ClientCredential.Password = credential.Password;
                newListsSoapClient.ClientCredentials.Windows.ClientCredential.Domain = credential.DomainName;
            }

            newListsSoapClient.ClientCredentials.Windows.AllowedImpersonationLevel =
                TokenImpersonationLevel.Impersonation;
            return newListsSoapClient;
        }

        public static WebsSoapClient GetWebsSoapClient(SiteSoapClientMapper mapper, ClearTextCredential credential)
        {
            var newWebsSoapClient = new WebsSoapClient();
            newWebsSoapClient.Endpoint.Address = new EndpointAddress(mapper.Url + "/_vti_bin/webs.asmx");
            if (newWebsSoapClient.ClientCredentials.Windows.ClientCredential == null)
            {
                newWebsSoapClient.ClientCredentials.Windows.ClientCredential = new NetworkCredential(
                    credential.Username, credential.Password, credential.DomainName);
            }
            else
            {
                newWebsSoapClient.ClientCredentials.Windows.ClientCredential.UserName = credential.Username;
                newWebsSoapClient.ClientCredentials.Windows.ClientCredential.Password = credential.Password;
                newWebsSoapClient.ClientCredentials.Windows.ClientCredential.Domain = credential.DomainName;
            }
            //newWebsSoapClient.ClientCredentials.Windows.ClientCredential = new NetworkCredential(credential.Username, credential.Password, credential.DomainName);
            newWebsSoapClient.ClientCredentials.Windows.AllowedImpersonationLevel =
                TokenImpersonationLevel.Impersonation;
            return newWebsSoapClient;
        }

        public static CopySoapClient GetCopySoapClient(SiteSoapClientMapper mapper, ClearTextCredential credential)
        {
            var newCopySoapClient = new CopySoapClient();
            newCopySoapClient.Endpoint.Address = new EndpointAddress(mapper.Url + "/_vti_bin/copy.asmx");
            if (newCopySoapClient.ClientCredentials.Windows.ClientCredential == null)
            {
                newCopySoapClient.ClientCredentials.Windows.ClientCredential = new NetworkCredential(
                    credential.Username, credential.Password, credential.DomainName);
            }
            else
            {
                newCopySoapClient.ClientCredentials.Windows.ClientCredential.UserName = credential.Username;
                newCopySoapClient.ClientCredentials.Windows.ClientCredential.Password = credential.Password;
                newCopySoapClient.ClientCredentials.Windows.ClientCredential.Domain = credential.DomainName;
            }
            //newCopySoapClient.ClientCredentials.Windows.ClientCredential = new NetworkCredential(credential.Username, credential.Password, credential.DomainName);
            newCopySoapClient.ClientCredentials.Windows.AllowedImpersonationLevel =
                TokenImpersonationLevel.Impersonation;
            return newCopySoapClient;
        }

        public static VersionsSoapClient GetVersionsSoapClient(SiteSoapClientMapper mapper, ClearTextCredential credential)
        {
            var versionsSoapClient = new VersionsSoapClient();
            versionsSoapClient.Endpoint.Address = new EndpointAddress(mapper.Url + "/_vti_bin/Versions.asmx");
            if (versionsSoapClient.ClientCredentials.Windows.ClientCredential == null)
            {
                versionsSoapClient.ClientCredentials.Windows.ClientCredential = new NetworkCredential(
                    credential.Username, credential.Password, credential.DomainName);
            }
            else
            {
                versionsSoapClient.ClientCredentials.Windows.ClientCredential.UserName = credential.Username;
                versionsSoapClient.ClientCredentials.Windows.ClientCredential.Password = credential.Password;
                versionsSoapClient.ClientCredentials.Windows.ClientCredential.Domain = credential.DomainName;
            }
            //newCopySoapClient.ClientCredentials.Windows.ClientCredential = new NetworkCredential(credential.Username, credential.Password, credential.DomainName);
            versionsSoapClient.ClientCredentials.Windows.AllowedImpersonationLevel =
                TokenImpersonationLevel.Impersonation;
            return versionsSoapClient;
        }
    }
}
using System;
using Core;
using Entities;
using Main.Services.Domains;
using Microsoft.SharePoint.Client;
using Services;

namespace Main.Services
{
    public class SharepointServiceProvider
    {
        public static ISharePointService GetSharePointService(SiteSoapClientMapper key, SharePointServerVersion serverVersion, ClearTextCredential credential)
        {
            switch (serverVersion)
            {
                case SharePointServerVersion.SharePoint2007:
                    return new SharePoint2007Service(SoapClient2007Repo.GetWebsSoapClient(key, credential),
                        SoapClient2007Repo.GetListsSoapClient(key, credential),
                        SoapClient2007Repo.GetCopySoapClient(key, credential),
                        SoapClient2007Repo.GetVersionsSoapClient(key, credential),
                        SoapClient2007Repo.GetUserProfileService(key, credential),
                        SoapClient2007Repo.GetSitesSoapClient(key, credential),
                        SoapClient2007Repo.GetSiteDataSoapClient(key, credential),
                        credential, key, serverVersion);
                case SharePointServerVersion.SharePoint2010:
                    return new SharePoint2010ServiceClientObjModel(new ClientContext(key.Url), credential, key, serverVersion);
                    //return new SharePoint2010Service(SoapClient2010Repo.GetWebsSoapClient(key, credential),
                    //    SoapClient2010Repo.GetListsSoapClient(key, credential),
                    //    SoapClient2010Repo.GetCopySoapClient(key, credential),
                    //    SoapClient2010Repo.GetVersionsSoapClient(key, credential), credential, key, serverVersion);
                case SharePointServerVersion.SharePoint2013:
                    return new SharePointService(new ClientContext(key.Url), credential, key, serverVersion);
                case SharePointServerVersion.SharePointOnline:
                    return new SharePointOnlineService(new ClientContext(key.Url), credential, key, serverVersion);
                default:
                    throw new ArgumentOutOfRangeException("serverVersion");
            }
        }
    }
}

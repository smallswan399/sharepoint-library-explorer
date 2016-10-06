using System;
using Core;
using Main.Services;
using Main.Services.Domains;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var service = SharepointServiceProvider.GetSharePointService(new SiteSoapClientMapper()
            {
                Id = 1,
                Url = "http://65.254.46.34:8001/",
                RootUrl = "http://65.254.46.34:8001/"
            },
                SharePointServerVersion.SharePoint2013, new ClearTextCredential()
                {
                    DomainName = "",
                    Password = "@SharePoint2007!Admin",
                    PrivateActiveDirectory = true,
                    ReTypePassword = "@SharePoint2007!Admin",
                    SharePointAuthenticationOption = SharePointAuthenticationOption.RequireNormalAuthentication,
                    Username = "sp2007admin"
                });


        }
    }
}

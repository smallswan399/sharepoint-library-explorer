using System;
using Core;
using Entities;
using EponaExtension;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EponaExtensionTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void GetAddtionalSitesTest()
        {
            var ex = new GetAddtionalEponaSiteCollection();
            ex.GetAddtionalSites(new Site()
            {
                Url = "http://dmsflcourse001.epona.com/",
                Credential = new Credential()
                {
                    Username = @"COURSE001\spsservice",
                    Password = "DMS4Legal",
                    SharePointAuthenticationOption = SharePointAuthenticationOption.RequireNormalAuthentication
                },
                RequireAuthentication = true,
                Enable = true,
                SharePointServerVersion = SharePointServerVersion.SharePoint2013
            });


        }
    }
}

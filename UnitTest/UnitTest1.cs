using System;
using Core.Libs;
using Main.Libs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetRootUrl_url_and_url1_dont_end_with_slash()
        {
            var expectResult = "http://stackoverflow.com";
            var absoluteUrl1 = "http://stackoverflow.com/questions/3681052/get-absolute-url-from-relative-path-refactored-method";
            var relativeUrl1 = "/questions/3681052/get-absolute-url-from-relative-path-refactored-method";

            var absoluteUrl2 = "http://stackoverflow.com/questions/3681052/get-absolute-url-from-relative-path-refactored-method";
            var relativeUrl2 = "/questions/3681052/get-absolute-url-from-relative-path-refactored-method/";

            var absoluteUrl3 = "http://stackoverflow.com/questions/3681052/get-absolute-url-from-relative-path-refactored-method";
            var relativeUrl3 = "questions/3681052/get-absolute-url-from-relative-path-refactored-method";

            var absoluteUrl4 = "http://stackoverflow.com/questions/3681052/get-absolute-url-from-relative-path-refactored-method";
            var relativeUrl4 = "questions/3681052/get-absolute-url-from-relative-path-refactored-method/";


            var absoluteUrl5 = "http://stackoverflow.com/questions/3681052/get-absolute-url-from-relative-path-refactored-method/";
            var relativeUrl5 = "/questions/3681052/get-absolute-url-from-relative-path-refactored-method";

            var absoluteUrl6 = "http://stackoverflow.com/questions/3681052/get-absolute-url-from-relative-path-refactored-method/";
            var relativeUrl6 = "/questions/3681052/get-absolute-url-from-relative-path-refactored-method/";

            var absoluteUrl7 = "http://stackoverflow.com/questions/3681052/get-absolute-url-from-relative-path-refactored-method/";
            var relativeUrl7 = "questions/3681052/get-absolute-url-from-relative-path-refactored-method";

            var absoluteUrl8 = "http://stackoverflow.com/questions/3681052/get-absolute-url-from-relative-path-refactored-method/";
            var relativeUrl8 = "questions/3681052/get-absolute-url-from-relative-path-refactored-method/";


            // var rootUrl = Utils.GetRootUrl(url, url1);
            Assert.AreEqual(Utils.GetRootUrl(absoluteUrl1, relativeUrl1), expectResult);
            Assert.AreEqual(Utils.GetRootUrl(absoluteUrl2, relativeUrl2), expectResult);
            Assert.AreEqual(Utils.GetRootUrl(absoluteUrl3, relativeUrl3), expectResult);
            Assert.AreEqual(Utils.GetRootUrl(absoluteUrl4, relativeUrl4), expectResult);
            Assert.AreEqual(Utils.GetRootUrl(absoluteUrl5, relativeUrl5), expectResult);
            Assert.AreEqual(Utils.GetRootUrl(absoluteUrl6, relativeUrl6), expectResult);
            Assert.AreEqual(Utils.GetRootUrl(absoluteUrl7, relativeUrl7), expectResult);
            Assert.AreEqual(Utils.GetRootUrl(absoluteUrl8, relativeUrl8), expectResult);
        }

        [TestMethod]
        public void AAA()
        {
            var url1 = "http://stackoverflow.com/";
            var url2 = "/questions/3681052/";
            var url3 = "/get-absolute-url-from-relative-path-refactored-method/";

            var ex = "http://stackoverflow.com/questions/3681052/get-absolute-url-from-relative-path-refactored-method";
            var result = url1.CombineUrl(url2, url3);
            Assert.AreEqual(ex, result);

        }

        //[TestMethod]
        //public void GetRootUrl_url_end_with_slash_url1_doesnt_end_with_slash()
        //{
        //    var url = "http://stackoverflow.com/questions/3681052/get-absolute-url-from-relative-path-refactored-method/";
        //    var url1 = "/questions/3681052/get-absolute-url-from-relative-path-refactored-method";
        //    var expectResult = "http://stackoverflow.com";


        //    var rootUrl = Utils.GetRootUrl(url, url1);
        //    Assert.AreEqual(rootUrl, expectResult);
        //}

        //[TestMethod]
        //public void GetRootUrl_url_end_with_slash_url1_end_with_slash()
        //{
        //    var url = "http://stackoverflow.com/questions/3681052/get-absolute-url-from-relative-path-refactored-method/";
        //    var url1 = "/questions/3681052/get-absolute-url-from-relative-path-refactored-method/";
        //    var expectResult = "http://stackoverflow.com";


        //    var rootUrl = Utils.GetRootUrl(url, url1);
        //    Assert.AreEqual(rootUrl, expectResult);
        //}

        //[TestMethod]
        //public void GetRootUrl_url_no_end_with_slash_url1_end_with_slash()
        //{
        //    var url = "http://stackoverflow.com/questions/3681052/get-absolute-url-from-relative-path-refactored-method";
        //    var url1 = "/questions/3681052/get-absolute-url-from-relative-path-refactored-method/";
        //    var expectResult = "http://stackoverflow.com";


        //    var rootUrl = Utils.GetRootUrl(url, url1);
        //    Assert.AreEqual(rootUrl, expectResult);
        //}
    }
}

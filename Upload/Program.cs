using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upload
{
    public class Program
    {
        static void Main()
        {
            var date1 = "12/6/2014 8:26 AM";
            var date2 = "01/27/2015 11:28 AM";

            var dateTime = DateTime.ParseExact(date1, "M/dd/yyyy h:mm tt", null);
            var dateTime2 = DateTime.ParseExact(date2, "M/dd/yyyy h:mm tt", null);
            Console.ReadLine();

            //var docLibHelper = new DocLibHelper();
            //var properties = new Dictionary<string, object> {{"Title", "Test Title" + DateTime.Now}};
            ////Create or overwrite text file test.txt in 'Docs' document library creating folder 'Test Folder' as required.
            //docLibHelper.Upload("http://65.254.46.34:8001/Shared Documents/c1111/test.txt", Encoding.ASCII.GetBytes("Test text.changes"), properties);
            //Console.WriteLine("Done");
            //Console.ReadLine();
        }
    }
}

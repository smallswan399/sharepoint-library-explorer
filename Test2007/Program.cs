using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2007
{
    class Program
    {
        static void Main(string[] args)
        {
            DocLibHelper docLibHelper = new DocLibHelper();
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("Title", "Test Title");
            //Create or overwrite text file test.txt in 'Docs' document library creating folder 'Test Folder' as required.
            docLibHelper.Upload("http://localhost/Docs/Test Folder/test.txt", System.Text.Encoding.ASCII.GetBytes("Test text."), properties);
        }
    }
}

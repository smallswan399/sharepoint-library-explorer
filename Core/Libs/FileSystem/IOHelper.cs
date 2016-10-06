using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.Libs.FileSystem
{
    public static class IoHelper
    {
        public static string ReadTextFile(string filePath)
        {
            var result = string.Empty;
            if (!File.Exists(filePath))
            {
                //ExceptionHandler.Handle(new Exception("File not found:" + filePath));
                return result;
            }
            try
            {
                var file = new StreamReader(filePath);
                result = file.ReadToEnd();
                file.Close();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return result;
        }

        public static List<string> SearchDirectory(string searchPattern, string directoryName)
        {
            //Declare a List of Strings for Returning File Names
            var strFileNames = new List<string>();
            //Get the Directory Info using Directory Name
            var dirInfor = new DirectoryInfo(directoryName);
            // Get all files whose names with searchPattern
            var filesInfo = dirInfor.GetFiles(searchPattern, SearchOption.TopDirectoryOnly);
            //Loop through all the files found and add to 
            //List and return them
            strFileNames.AddRange(filesInfo.Select(fi => fi.FullName));
            return strFileNames;
        }
    }
}

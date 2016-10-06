using System;

namespace Core
{
    public static class Constants
    {
        public const string KeyName = @"HKEY_CURRENT_USER\Software\Litera2\SharePoint\";
        public const string KeyName2 = @"HKEY_CURRENT_USER\Software\Litera2\Common3";
        public const string SceretString = "+)4ij |_4` m0.t chu0~j h3't su'c phu'c t4.p";
        public const string MissDownloadDirectory =
            "Download directory is missing. Please specify the download directory!";

        public const string DownloadDirectoryNotExist = "Download directory does not exist!";
        public static string Data
        {
            get
            {
                // if Data = empty, => use current directory
                //return "";
                var sDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Litera\SharePoint";
                return sDir;
            }
        }
    }
}

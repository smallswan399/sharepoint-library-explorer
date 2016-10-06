using System;
using System.Threading;
using Core;
using Core.Libs;
using RGiesecke.DllExport;

namespace Main
{
    public class NativeExports
    {
        [DllExport("config")]
        public static void Config(int key, int handle)
        {
            var handler = new IntPtr(handle);
            var owner = new WindowWrapper(handler);
            var keyString = Constants.KeyName + key;
            var repoProvider = Program.GetRepoProvider(keyString);

            var t = new Thread(() =>
            {
                Program.Config(repoProvider, owner);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        [DllExport("loadfromdms")]
        public static void LoadFromDms(int key, int handle)
        {
            var handler = new IntPtr(handle);
            var owner = new WindowWrapper(handler);
            var keyString = Constants.KeyName + key;
            var repoProvider = Program.GetRepoProvider(keyString);

            var t = new Thread(() =>
            {
                Log.LogMessage("Trying " + key);
                Program.LoadFromDms(repoProvider, owner);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        [DllExport("downloadfromdms")]
        public static void DownloadFromDms(int key)
        {
            var keyString = Constants.KeyName + key;
            var repoProvider = Program.GetRepoProvider(keyString);

            Log.LogMessage("Trying " + key);
            // the local directory is stored in data.xml, can change this data to change to a new folder will be used to stored downloaded files
            Program.DownloadFromDms(repoProvider);
        }


        [DllExport("savenewprofile")]
        public static void SaveNewProfile(int key, int handle)
        {
            var handler = new IntPtr(handle);
            var owner = new WindowWrapper(handler);
            var keyString = Constants.KeyName + key;
            var repoProvider = Program.GetRepoProvider(keyString);

            var t = new Thread(() =>
            {
                Log.LogMessage("Trying " + key);
                Program.UploadFile(repoProvider, owner);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        [DllExport("savenewprofilesilent")]
        public static void SaveNewProfileSilent(int key)
        {
            var keyString = Constants.KeyName + key;
            var repoProvider = Program.GetRepoProvider(keyString);

            Log.LogMessage("Trying " + key);
            Program.UploadAsNewProfileSilent(repoProvider);
        }

        [DllExport("savenewversion")]
        public static void SaveNewVersion(int key, int handle)
        {
            var keyString = Constants.KeyName + key;
            var repoProvider = Program.GetRepoProvider(keyString);

            Log.LogMessage("Trying " + key);
            Program.UploadFileVersion(repoProvider);
        }
    }
}

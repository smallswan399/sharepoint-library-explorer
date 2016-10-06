using System.Collections.Generic;
using System.Linq;
using Main.Core;
using Main.Libs;
using Main.Properties;
using Main.Repos;
using Main.Services;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Core;
using Core.Libs;
using Entities;
using Services;

namespace Main
{
    public static class Program
    {
        ///// <summary>
        ///// The main entry point for the application.
        ///// </summary>
        //[STAThread]
        public static void Main(string[] args)
        {
            Log.LogMessage("===================================================");
            Log.LogMessage("Start the app with the args = " + CmdHelper.GetString(args));

            var options = new CmdParameters();
            var handle = IntPtr.Zero;

            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                // consume Options instance properties
                if (options.Handle != null)
                {
                    handle = new IntPtr((long)options.Handle);
                }
            }
            var owner = new WindowWrapper(handle);
            try
            {
                // Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                UpdateRegistry();
                if (!CheckDataDir()) return;
                var key = Constants.KeyName;
                if (args.Length < 2 || string.IsNullOrWhiteSpace(args[1]))
                {
                    key += "0";
                }
                else
                {
                    key += args[1];
                }
                var repoProvider = GetRepoProvider(key);

                // run directly, have no parameters
                if (!args.Any())
                {
                    Log.LogMessage("Run the app directly, have no cmd parameters");
                    new MainForm(repoProvider).ShowDialog(owner);
                }
                else
                {
                    var param0 = args[0].ToLower();

                    // open sharepoint sites manager
                    if (param0 == "config")
                    {
                        (new SitesForm(repoProvider)).ShowDialog(owner);
                    }

                    if (param0 == "otest")
                    {
                        MessageBox.Show(@"SharePoint InterOp is working", @"Success", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    if (param0 == "about")
                    {
                        var aboutForm = new About();
                        aboutForm.ShowDialog(owner);
                    }
                    #region loadfromdms
                    // get file url then store the value to data.xml. This information will be used to download these files later
                    if ((param0 == "loadfromdms") || (param0 == "loaddms"))
                    {
                        Log.LogMessage("Trying " + param0);
                        LoadFromDms(repoProvider, owner);
                    }
                    #endregion

                    #region downloadfromdms

                    // download files that were selected and stored the Urls in data.xml. Store in the local, save local path of the files in data.xml
                    // the external can access to this data
                    if ((param0 == "downloadfromdms") || (param0 == "download"))
                    {
                        Log.LogMessage("Trying " + param0);
                        // the local directory is stored in data.xml, can change this data to change to a new folder will be used to stored downloaded files
                        DownloadFromDms(repoProvider);
                    }

                    #endregion

                    #region savenewprofile

                    // upload a file to the server. The uploading file have to be not existing on the server
                    if (param0 == "uploadtodms" || (param0 == "savenewprofile") || (param0 == "savenew") || (param0 == "saveprofile") || (param0 == "save") || (param0 == "new"))
                    {
                        Log.LogMessage("Trying " + param0);
                        UploadFile(repoProvider, owner);
                    }

                    #endregion

                    #region SaveNewProfileSilent

                    if (param0 == "savenewprofilesilent")
                    {
                        Log.LogMessage("Trying " + param0);
                        UploadAsNewProfileSilent(repoProvider);

                    }

                    #endregion


                    #region Save new version
                    // upload a file to the server. The app will handle new file or a version of an exist file
                    if ((param0 == "savenewversion") || (param0 == "savever") || (param0 == "savenewver"))
                    {
                        Log.LogMessage("Trying " + param0);
                        UploadFileVersion(repoProvider);
                    }
                    #endregion



                    #region getallversion

                    // can get all history version of a file in Main form
                    // Do nothing

                    #endregion

                    Log.LogMessage("Close the app");
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }

        public static IRepoProvider GetRepoProvider(string key)
        {
            if (Settings.Default.NewDataSchema)
            {
                Log.LogMessage("Use NewDataSchema");
                return new RepoProvider(Constants.Data);
            }
            Log.LogMessage("Use OldDataSchema, Registry to use at " + key);
            return new OldSchemaRepoProvider(key);
        }

        public static void UploadAsNewProfileSilent(IRepoProvider repoProvider)
        {
            repoProvider.UploadedFiles.Result.Status = ResultStatus.Pending;

            var uploadingFiles = repoProvider.UploadingFiles;
            if (!uploadingFiles.Files.Any())
            {
                MessageBox.Show(@"There are no files to upload", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.LogMessage(@"There are no files to upload", 1);
                repoProvider.SaveChanges(ResultMode.UploadFiles);
                return;
            }
            if (uploadingFiles.Files.First().VersionHolderSelectedFileFile == null)
            {
                MessageBox.Show(@"Have not yet specified a file on server", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.LogMessage(@"Have not yet specified a file on server", 1);
                repoProvider.SaveChanges(ResultMode.UploadFiles);
                return;
            }

            // Check local file
            var localFile = uploadingFiles.Files.First().LocalPath;
            if (!File.Exists(localFile))
            {
                Log.LogException(new FileNotFoundException(localFile + " not found"));
                MessageBox.Show(localFile + @" does not exist", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                repoProvider.SaveChanges(ResultMode.UploadFiles);
                return;
            }

            // Check remote file name
            var remoteFile = uploadingFiles.Files.First().VersionHolderSelectedFileFile;
            var remoteFileName = remoteFile.Name;
            var validationResult = MainForm.ValidateFileName(remoteFileName);
            if (!string.IsNullOrWhiteSpace(validationResult))
            {
                MessageBox.Show(validationResult, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.LogMessage("Invalid file name", 1);
                repoProvider.SaveChanges(ResultMode.UploadFiles);
                return;
            }

            //Log.LogMessage(string.Format("Upload file as new profile, {0} - {1}", localFile,
            //    remoteFile.IsCurrentVersion ? remoteFile.Url : remoteFile.FileVersionUrl));

            // Get SharePointService from remote file
            var site = repoProvider.SiteRepository.GetByIdRecursive(remoteFile.LocalId);

            if (site == null)
            {
                MessageBox.Show(@"Cant specific a server to upload to", @"Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Log.LogException(new Exception("Can't specific the destination server."));
                repoProvider.SaveChanges(ResultMode.UploadFiles);
                return;
            }

            var sharePointService =
                SharepointServiceProvider.GetSharePointService(new SiteSoapClientMapper()
                {
                    Id = site.Id,
                    RootUrl = site.RootUrl,
                    Url = site.Url
                }, site.SharePointServerVersion, site.Credential.ToClearTextCredential());

            if (sharePointService == null)
            {
                MessageBox.Show(@"Cant specific a server to upload to", @"Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Log.LogException(new Exception("Can't specific the destination server."));
                repoProvider.SaveChanges(ResultMode.UploadFiles);
                return;
            }

            try
            {
                Log.LogMessage("Check existing", 1);
                var sharePointFile = sharePointService.GetSharePointFile(remoteFile.Url);
                // The file was exist
                if (sharePointFile != null)
                {
                    MessageBox.Show(@"The file is existing.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.LogMessage("File exists");
                    repoProvider.SaveChanges(ResultMode.UploadFiles);
                    return;
                }

                var fileStream = new FileStream(localFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                // Clear uploaded files from previous session
                repoProvider.UploadedFiles.Files.Clear();
                // Set upload process is Pending
                repoProvider.UploadedFiles.Result.Status = ResultStatus.Pending;
                // Save changes
                repoProvider.SaveChanges(ResultMode.UploadFiles);

                // Upload file
                var result = sharePointService.UploadFile(remoteFile.Url, fileStream);

                if (result.All(t => t.ErrorCode == ErrorCode.Success))
                {
                    Log.LogMessage("Upload successfully file as new profile", 1);
                    Log.LogMessage("Local file: " + localFile, 2);
                    Log.LogMessage("Remote file: " + remoteFile.Url, 2);

                    // var uploadedUrl = result.First(t => t.ErrorCode == ErrorCode.Success).Url;
                    // add current file upload result to the data.xml
                    repoProvider.UploadedFiles.Files.Add(new UploadedFile()
                    {
                        LocalId = site.Id,
                        Url = result.First().Url,
                        Name = remoteFileName,
                        LocalPath = localFile,
                        SiteUrl = site.Url,
                        Uploaded = true
                    });
                    repoProvider.UploadedFiles.Result.Status = ResultStatus.Success;
                    repoProvider.SaveChanges(ResultMode.UploadFiles);
                    var parentList = sharePointService.GetParentSharePointLibraryByFileUrl(remoteFile.Url);
                    if (parentList.RequireCheckout)
                    {
                        Log.LogMessage("Check-in file", 2);
                        if (parentList.EnableVersioning && (parentList.GetAncestorSharePointLibrary().EnableMinorVersions))
                        {
                            sharePointService.CheckInFile(remoteFile.Url, "", "0");
                        }
                        sharePointService.CheckInFile(remoteFile.Url, "", "1");
                    }
                }
                else
                {
                    result.ToList().ForEach(s => Log.LogMessage(s.Message, 2));
                }
                fileStream.Close();
            }
            catch (Exception ex)
            {
                repoProvider.SaveChanges(ResultMode.UploadFiles);
                Log.LogException(ex);
            }
        }

        public static void DownloadFromDms(IRepoProvider repoProvider)
        {
            try
            {
                repoProvider.DownloadedFiles.Result.Status = ResultStatus.Pending;
                repoProvider.DownloadedFiles.Files.Clear();
                repoProvider.SaveChanges(ResultMode.DownloadFiles);

                if (repoProvider.SelectedFiles.Result.Status == ResultStatus.Success)
                {
                    // dispaly form that user can set folder to store files
                    var containFolder = repoProvider.DownloadDirectory;
                    if (string.IsNullOrWhiteSpace(containFolder))
                    {
                        MessageBox.Show(Constants.MissDownloadDirectory, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    var folder = repoProvider.DownloadDirectory;
                    if (!Directory.Exists(folder))
                    {
                        MessageBox.Show(Constants.DownloadDirectoryNotExist, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // get file urls that are stored in data.xml
                    var fileUrls = repoProvider.SelectedFiles.Files;
                    // store the local path of downloaded files
                    var sharePointServices = new List<ISharePointService>();
                    // reset DownloadedFiles
                    repoProvider.DownloadedFiles = new DownloadedFiles()
                    {
                        Result = new Result() { Status = ResultStatus.Pending }
                    };
                    repoProvider.SaveChanges(ResultMode.DownloadFiles);

                    fileUrls.ForEach(s =>
                    {   
                        var existSharePointService =
                            sharePointServices.FirstOrDefault(
                                t =>
                                    t.SiteSoapClientMapper.Id == s.LocalId &&
                                    t.SiteSoapClientMapper.Url == s.SiteUrl);
                        if (existSharePointService == null)
                        {
                            var site = repoProvider.SiteRepository.GetByIdRecursive(s.LocalId);
                            if (site == null)
                            {
                                MessageBox.Show(@"The server does not exist", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Log.LogException(new Exception("Server " + s.LocalId + " not found"));
                                return;
                            }

                            existSharePointService =
                                SharepointServiceProvider.GetSharePointService(new SiteSoapClientMapper()
                                {
                                    Id = site.Id,
                                    RootUrl = site.RootUrl,
                                    Url = s.SiteUrl
                                }, site.SharePointServerVersion, site.Credential.ToClearTextCredential());
                            sharePointServices.Add(existSharePointService);
                        }

                        // If is current version, download the file from Url, if not, download the file from FileVersionUrl
                        using (var fileInfo = s.IsCurrentVersion ? existSharePointService.DownloadFileByUrl(s.Url) : existSharePointService.DownloadHistoryVersion(s.FileVersionUrl))
                        {
                            Log.LogMessage("Downloaded file", 1);
                            if (s.IsCurrentVersion)
                            {
                                Log.LogMessage("Remote: " + s.Url, 2);
                            }
                            else
                            {
                                Log.LogMessage("Remote: " + s.FileVersionUrl, 2);
                            }
                            Log.LogMessage("Local: " + Path.Combine(folder, Path.GetFileNameWithoutExtension(s.Name) + "_" + s.UiVersionLabel + Path.GetExtension(s.Name)), 2);
                            using (
                                var fs =
                                    new FileStream(
                                        Path.Combine(folder,
                                            Path.GetFileNameWithoutExtension(s.Name) + "_" + s.UiVersionLabel +
                                            Path.GetExtension(s.Name)), FileMode.OpenOrCreate))
                            {
                                Utils.CopyTo(fileInfo, fs);
                            }
                            
                            // repoProvider.DownloadedFiles.Files.Clear();
                            // update DownloadedFiles list for each success downloading
                            repoProvider.DownloadedFiles.Files.Add(new DownloadedFile()
                            {
                                Downloaded = true,
                                LocalId = s.LocalId,
                                LocalPath = Path.Combine(folder, Path.GetFileNameWithoutExtension(s.Name) + "_" + s.UiVersionLabel + Path.GetExtension(s.Name)),
                                Name = s.Name,
                                SiteUrl = s.SiteUrl,
                                Url = s.Url
                            });
                            repoProvider.DownloadedFiles.Result.Status = ResultStatus.Success;
                            repoProvider.SaveChanges(ResultMode.DownloadFiles);
                        }
                    });
                    repoProvider.DownloadedFiles.Result = new Result() { Status = ResultStatus.Success };
                    repoProvider.SaveChanges(ResultMode.DownloadFiles);
                }
                else
                {
                    Log.LogMessage("Cant not specify which file need to be downloaded.", 1);
                }
            }

            catch (Exception ex)
            {
                repoProvider.DownloadedFiles.Result.Status = ResultStatus.Error;
                repoProvider.DownloadedFiles.Result.Message = ex.Message;
                repoProvider.SaveChanges(ResultMode.DownloadFiles);
                Log.LogException(ex);
            }
        }

        public static void LoadFromDms(IRepoProvider repoProvider, WindowWrapper owner)
        {
            try
            {
                var form = new MainForm(repoProvider, FormAction.GetUrl);
                form.ShowDialog(owner);
            }
            catch (Exception ex)
            {
                repoProvider.SelectedFiles.Result.Status = ResultStatus.Error;
                repoProvider.SaveChanges();
                Log.LogException(ex);
            }
        }

        /// <summary>
        /// Upload file to the server as new profile
        /// </summary>
        /// <param name="repoProvider"></param>
        /// <param name="owner"></param>
        public static void UploadFile(IRepoProvider repoProvider, WindowWrapper owner)
        {
            try
            {
                var form = new MainForm(repoProvider, FormAction.Upload);
                form.ShowDialog(owner);
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }

        public static void UploadFileVersion(IRepoProvider repoProvider)
        {
            // Log.LogMessage("Upload file as new version");
            var form = new MainForm(repoProvider, FormAction.UploadVersion);
            form.UploadFileVersionHandlerv2();
        }

        public static void UpdateRegistry()
        {
            Log.LogMessage(@"Update registry Key: HKEY_CURRENT_USER\Software\Litera2\Common3\FilePathSharePoint, Vaule: " + Application.ExecutablePath);
            Registry.SetValue(Constants.KeyName2, "FilePathSharePoint", Application.ExecutablePath, RegistryValueKind.String);
        }

        /// <summary>
        /// Check the folder that contain data
        /// </summary>
        /// <returns></returns>
        public static bool CheckDataDir()
        {
            try
            {
                var dbFolder = Constants.Data;
                
                if (!Directory.Exists(dbFolder))
                {
                    Directory.CreateDirectory(dbFolder);
                    Log.LogMessage("Create a directory to store the app data at " + dbFolder);
                }
                Log.LogMessage("Check CheckDataDir successfully");
                return true;
            }
            catch (Exception ex)
            {
                Log.LogMessage("Check CheckDataDir failed");
                MessageBox.Show(ex.Message);
                Log.LogException(ex);

                return false;
            }
        }
        public static void Config(IRepoProvider repoProvider, WindowWrapper owner)
        {
            if (System.Diagnostics.Debugger.IsAttached == false)
                System.Diagnostics.Debugger.Launch();
            (new SitesForm(repoProvider)).ShowDialog(owner);
        }
    }
}

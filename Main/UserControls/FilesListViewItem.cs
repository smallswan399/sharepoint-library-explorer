using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entities;
using Main.Repos;
using Main.Services;
using Main.Services.Domains;
using SharePointFile = Main.Services.Domains.SharePointFile;

namespace Main.UserControls
{
    public class FilesListViewItem : ListViewItem
    {
        public ISharePointLibraryItem SharePointLibraryItem { get; set; }
        public ISharePointService SharePointService { get; private set; }
        private readonly IRepoProvider repoProvider;
        public FilesListViewItem(string[] items, int imageIndex, IRepoProvider repoProvider, ISharePointService sharePointService)
            : base(items, imageIndex)
        {
            this.repoProvider = repoProvider;
            SharePointService = sharePointService;
        }

        #region This is commented
        ///// <summary>
        ///// Handle when a FilesListViewItem is double click
        ///// </summary>
        ///// <param name="dir"></param>
        //public void DoubleClickHandler(string dir)
        //{
        //    if (SharePointLibraryItem is SharePointFolder)
        //    {
        //        var node =
        //            (ListView as FilesListView).SiteNodeTreeNode.Nodes.Cast<SiteNodeTreeNode>()
        //                .First(s => s.Text == SharePointLibraryItem.Name);
        //        node.TreeView.SelectedNode = node;
        //        node.Expand();
        //        return;
        //    }
        //    if (SharePointLibraryItem is SharePointFile)
        //    {
        //        #region Download doubleClick file
        //        //repoProvider.DownloadedFiles.Result.Status = ResultStatus.Pending;
        //        //repoProvider.DownloadedFiles.Files.Clear();
        //        //repoProvider.SaveChanges(ResultMode.DownloadFiles);
        //        //DownloadFile(dir);
        //        //MessageBox.Show(@"Download file successfully", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        //repoProvider.DownloadedFiles.Result.Status = ResultStatus.Success;
        //        //repoProvider.SaveChanges(ResultMode.DownloadFiles); 
        //        #endregion
        //        return;
        //    }

        //    MessageBox.Show(@"Error", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //} 
        #endregion

        /// <summary>
        /// Download the file that associated with the listview item
        /// </summary>
        /// <param name="dir"></param>
        public async Task DownloadFileAsync(string dir = "")
        {
            var sharePointFile = SharePointLibraryItem as SharePointFile;
            if (sharePointFile != null)
            {
                // Download file
                if (string.IsNullOrWhiteSpace(dir))
                {
                    var saveDiglog = new SaveFileDialog()
                    {
                        AddExtension = true,
                        Title = @"Download",
                        FileName = SharePointLibraryItem.Name,
                        Filter = @"Office Document|*.doc;*.docx;*.xls;*.xlsx;*.ppt;*.pptx|PDF|*.pdf|Image|*.png;*.jpg;*.jpeg;*.gif|Text Files|*.txt|All|*.*",
                        SupportMultiDottedExtensions = false
                    };

                    #region MyRegion
                    var fileEx = Path.GetExtension(SharePointLibraryItem.Name);
                    fileEx = string.IsNullOrWhiteSpace(fileEx) ? "" : fileEx.ToLower();
                    var ffficeExtensions = new List<string>
                    {
                        ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx"
                    };
                    var pdfExtensions = new List<string> { ".pdf" };
                    var imageExtensons = new List<string> { ".png", ".jpg", ".jpeg", ".gif" };
                    var textExtensions = new List<string> { ".txt" };

                    if (ffficeExtensions.Contains(fileEx))
                    {
                        saveDiglog.FilterIndex = 1;
                    }
                    else
                    {
                        if (pdfExtensions.Contains(fileEx))
                        {
                            saveDiglog.FilterIndex = 2;
                        }
                        else
                        {
                            if (imageExtensons.Contains(fileEx))
                            {
                                saveDiglog.FilterIndex = 3;
                            }
                            else
                            {
                                saveDiglog.FilterIndex = textExtensions.Contains(fileEx) ? 4 : 5;
                            }
                        }
                    } 
                    #endregion

                    //saveDiglog.FilterIndex = 

                    if (saveDiglog.ShowDialog() == DialogResult.OK)
                    {
                        var node = (ListView as FilesListView).SiteNodeTreeNode;
                        
                        // Not need to check out file before download file
                        // SharePointLibraryItem must be a SharePointFile
                        //if (sharePointFile.GetAncestorSharePointLibrary().RequireCheckout)
                        //{
                        //    // check checkout user to?
                        //    var user = sharePointFile.CheckoutUser;
                        //}

                        //var checkOutResult =
                        //    SharePointService.CheckOutFile(
                        //        SharePointLibraryItem.RootUrl + SharePointLibraryItem.RelativeUrl, true.ToString(),
                        //        SharePointLibraryItem.LastModifiedDateTime.ToString("r"));
                        //if (!checkOutResult)
                        //{
                        //    MessageBox.Show(@"Check out " + SharePointLibraryItem.RootUrl +
                        //                    SharePointLibraryItem.RelativeUrl +
                        //                    @" failed! There are no files were downloaded");
                        //    return;
                        //}

                        // Save the file was downloaded to the local PC
                        await node.DownloadFileAsync(saveDiglog.FileName, SharePointLibraryItem.RootUrl + SharePointLibraryItem.RelativeUrl);
                        // Update DownloadedFiles data
                        repoProvider.DownloadedFiles.Files.Add(new DownloadedFile()
                        {
                            Downloaded = true,
                            Name = SharePointLibraryItem.Name,
                            Url = SharePointLibraryItem.RootUrl + SharePointLibraryItem.RelativeUrl,
                            LocalId = SharePointLibraryItem.GetSharePointSiteLocalId(),
                            LocalPath = saveDiglog.FileName,
                            SiteUrl = SharePointLibraryItem.GetSharePointSiteUrl()
                        });
                        repoProvider.SaveChanges();
                        // MessageBox.Show(@"Success", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    var node = (ListView as FilesListView).SiteNodeTreeNode;
                    await node.DownloadFileAsync(Path.Combine(dir, SharePointLibraryItem.Name), SharePointLibraryItem.RootUrl + SharePointLibraryItem.RelativeUrl);
                    repoProvider.DownloadedFiles.Files.Add(new DownloadedFile()
                    {
                        Downloaded = true,
                        Name = SharePointLibraryItem.Name,
                        Url = SharePointLibraryItem.RootUrl + SharePointLibraryItem.RelativeUrl,
                        LocalId = SharePointLibraryItem.GetSharePointSiteLocalId(),
                        LocalPath = Path.Combine(dir, SharePointLibraryItem.Name),
                        SiteUrl = SharePointLibraryItem.GetSharePointSiteUrl()
                    });
                    repoProvider.SaveChanges();
                    // MessageBox.Show(@"Success", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }
            MessageBox.Show(@"Cannot download a Folder", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Check out result</returns>
        public async Task<bool> CheckOutAsync()
        {
            var sharePointFile = SharePointLibraryItem as SharePointFile;
            if (sharePointFile == null) throw new NotImplementedException();

            // Check the file was checked out to a user?
            if (!string.IsNullOrWhiteSpace(sharePointFile.CheckoutUser))
            {
                // check out to current user?
                // 1) get CheckoutUser
                // 2) get current user
                // 3) compare
                return sharePointFile.CheckoutUser == sharePointFile.User;
            }
            // Not yet checked out by a user
            var result = await SharePointService.CheckOutFileAsync(sharePointFile.RootUrl + sharePointFile.RelativeUrl, false.ToString(),
                sharePointFile.LastModifiedDateTime.ToString("r"));
            return result;
        }

        public async Task<bool> CheckInAsync()
        {
            if (CanCheckIn)
            {
                var sharePointFile = SharePointLibraryItem as SharePointFile;
                return await SharePointService.CheckInFileAsync(sharePointFile.RootUrl + sharePointFile.RelativeUrl, "", "1");
            }
            return false;
        }

        public string CheckOutUser
        {
            get
            {
                return SharePointLibraryItem is SharePointFile
                    ? ((SharePointFile) SharePointLibraryItem).CheckoutUser
                    : "";
            }
        }

        public bool CanCheckOut
        {
            get
            {
                if (SharePointLibraryItem is SharePointFolder)
                {
                    return false;
                }
                return string.IsNullOrWhiteSpace((SharePointLibraryItem as SharePointFile).CheckoutUser);
            }
        }

        public bool CanCheckIn
        {
            get
            {
                if (SharePointLibraryItem is SharePointFolder)
                {
                    return false;
                }
                return (SharePointLibraryItem as SharePointFile).CheckoutUser ==
                       (SharePointLibraryItem as SharePointFile).User;
            }
        }
    }
}

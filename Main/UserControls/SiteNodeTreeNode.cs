using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Libs;
using Main.Core;
using Main.Repos;
using Main.Services;
using Main.Services.Domains;
using Main.ViewModels;
using Services;

namespace Main.UserControls
{
    public class SiteNodeTreeNode : TreeNode
    {
        /// <summary>
        /// 
        /// </summary>
        public ContextMenuStrip SiteItemMenuStrip { get; set; }
        public ISharePointService SharePointService { get; set; }
        private readonly IRepoProvider repoProvider;
        public bool IsLoading { get; set; }

        public SiteNodeTreeNode
            (string text, int imageIndex, int selectedImageIndex, ISharePointService sharePointService, IRepoProvider repoProvider)
            : base(text, imageIndex, selectedImageIndex)
        {
            SharePointService = sharePointService;
            this.repoProvider = repoProvider;
            //NodeData = new List
        }

        public TreeNodeType TreeNodeType
        {
            get
            {
                if (SharePointExpandableObjcect is SharePointFolder)
                {
                    return TreeNodeType.Folder;
                }
                if (SharePointExpandableObjcect is SharePointLibrary)
                {
                    return TreeNodeType.Library;
                }
                if (SharePointExpandableObjcect is SharePointSite)
                {
                    return TreeNodeType.Site;
                }
                return TreeNodeType.Sites;
            }
        }

        /// <summary>
        /// SiteNodeTreeNode is a ISharePointExpandableObjcect (SharePointFolder, SharePointLibrary or SharePointSite)
        /// </summary>
        public ISharePointExpandableObjcect SharePointExpandableObjcect { get; set; }
        /// <summary>
        /// This is only for ISharePointLibraryItem node
        /// </summary>
        // public List<ISharePointLibraryItem> LibraryItems { get; set; }
        // This is only for the first level site nodes
        //public SiteDetails SiteDetails { get; set; }
        public Guid SiteId { get; set; }

        public bool HadFillDataFromRemote { get; set; }

        /// <summary>
        /// Expand the node after got the data from remote
        /// </summary>
        public async Task ExpandFromRemoteAsync()
        {
            switch (TreeNodeType)
            {
                // If node is a site, try get sub sites + all lists
                case TreeNodeType.Site:
                    await ExpandSiteNodeAsync();
                    break;
                case TreeNodeType.Library:
                    //IEnumerable<ISharePointFile> files;
                    await ExpandLibraryNodeAsync();
                    break;
                case TreeNodeType.Folder:
                    await ExpandFolderNodeAsync();
                    break;
                case TreeNodeType.Sites:
                    // this.Expand();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task InitRootSharePointSiteFromRemoteAsync()
        {
            if (TreeNodeType == TreeNodeType.Site)
            {
                Log.LogMessage("Init root SharePoint site from remote", 2);
                // Get SharePointSite from remote
                var site = await SharePointService.GetSharePointSiteAsync();
                // Init some data

                SharePointExpandableObjcect.RelativeUrl = site.RelativeUrl;
                // SharePointExpandableObjcect.RootUrl = site.RootUrl;
                SharePointExpandableObjcect.Title = site.Title;
                ((SharePointSite) SharePointExpandableObjcect).User = site.User;
                SharePointService.SiteSoapClientMapper.RootUrl = SharePointExpandableObjcect.RootUrl;

                var localSite = repoProvider.SiteRepository.GetById(SharePointService.SiteSoapClientMapper.Id);
                if (localSite != null)
                {
                    localSite.RootUrl = site.RootUrl;
                    repoProvider.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Download file using SharePointService
        /// </summary>
        /// <param name="target"></param>
        /// <param name="url">Absolute Url</param>
        public async Task DownloadFileAsync(string target, string url)
        {
            using (var fs = new FileStream(target, FileMode.OpenOrCreate))
            using (var fileInfo = await SharePointService.DownloadFileByUrlAsync(url))
            {
                await Utils.CopyToAsync(fileInfo, fs);
            }
        }

        /// <summary>
        /// Upload a file stream to the server
        /// </summary>
        /// <param name="streamFile"></param>
        /// <param name="name"></param>
        /// <param name="parentRelative"></param>
        /// <returns></returns>
        public async Task<List<CopyActionResult>> UploadFileAsync(Stream streamFile, string name, string parentRelative = "")
        {
            if (string.IsNullOrWhiteSpace(parentRelative))
            {
                parentRelative = SharePointExpandableObjcect.RelativeUrl;
            }
            var result = await SharePointService.UploadFileAsync(parentRelative, name, streamFile);
            // RefreshTreeNode();
            streamFile.Close();
            return result;
        }


        /// <summary>
        /// Refresh the selected site tree node from server
        /// </summary>
        public async Task RefreshTreeNodeAsync()
        {
            Nodes.Clear();
            if (TreeNodeType != TreeNodeType.Site)
            {
                ((ISharePointFileContainer) SharePointExpandableObjcect).SharePointLibraryItems.Clear();
                // LibraryItems.Clear();    
            }
            //ExpandableObjcect.
            HadFillDataFromRemote = false;
            await ExpandFromRemoteAsync();
            HadFillDataFromRemote = true;
        }

        /// <summary>
        /// Check in the file
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<bool> CheckinFileAsync(string url)
        {
            //string checkInType = string.Empty;
            if (((ISharePointFileContainer) SharePointExpandableObjcect).GetAncestorSharePointLibrary().EnableVersioning && ((ISharePointFileContainer) SharePointExpandableObjcect).GetAncestorSharePointLibrary().EnableMinorVersions)
            {
                return await SharePointService.CheckInFileAsync(url, "", "0");
            }
            return await SharePointService.CheckInFileAsync(url, "", "1");
        }

        #region Helper methods

        /// <summary>
        /// Call to expand a folder node
        /// </summary>
        private async Task ExpandFolderNodeAsync()
        {
            if (((ISharePointFileContainer) SharePointExpandableObjcect).SharePointLibraryItems == null)
            {
                ((ISharePointFileContainer) SharePointExpandableObjcect).SharePointLibraryItems = new List<ISharePointLibraryItem>();
            }
            var subFolders = await SharePointService.GetSharePointSubFolderAsync(((SharePointFolder) SharePointExpandableObjcect).RelativeUrl);
            subFolders = subFolders.OrderBy(s => s.Name);
            Log.LogMessage("Loaded subfolders in " + FullPath, 2);

            var files = await SharePointService.GetSharePointFilesInFolderAsync(((SharePointFolder) SharePointExpandableObjcect).RelativeUrl);
            files = files.OrderBy(s => s.Name);
            Log.LogMessage("Loaded files in " + FullPath, 2);

            subFolders.ToList().ForEach(folder =>
            {
                folder.ParentSharePointFolder = SharePointExpandableObjcect as SharePointFolder;
                // folder.RootUrl = folder.ParentSharePointFolder.RootUrl;
                // folder.Url = folder.RootUrl + folder.RelativeUrl;
                // (SharePointExpandableObjcect as SharePointFolder).SharePointLibraryItems.Add(folder);
                (((ISharePointFileContainer) SharePointExpandableObjcect).SharePointLibraryItems).Add(folder);
                // use SharePointService from parent node
                var node = new SiteNodeTreeNode(folder.Name, 9, 9, SharePointService, repoProvider)
                {
                    HadFillDataFromRemote = false,
                    SiteId = SiteId,
                    SharePointExpandableObjcect = folder,
                    ContextMenuStrip = SiteItemMenuStrip,
                    SiteItemMenuStrip = SiteItemMenuStrip
                };
                node.Nodes.Add("Dump Node");
                Nodes.Add(node);
            });
            files.ToList().ForEach(file =>
            {
                (((ISharePointFileContainer) SharePointExpandableObjcect).SharePointLibraryItems).Add(file);
                file.ParentSharePointFolder = SharePointExpandableObjcect as SharePointFolder;
                //file.RootUrl = file.ParentSharePointFolder.RootUrl;
                //file.Url = file.RootUrl + file.RelativeUrl;
            });
            HadFillDataFromRemote = true;
        }

        /// <summary>
        /// Call to expand a library node
        /// </summary>
        private async Task ExpandLibraryNodeAsync()
        {
            if (((ISharePointFileContainer) SharePointExpandableObjcect).SharePointLibraryItems == null)
            {
                ((ISharePointFileContainer) SharePointExpandableObjcect).SharePointLibraryItems = new List<ISharePointLibraryItem>();
            }

            // If node is a libraty, try get sub folder of the library
            var folders = await SharePointService.GetSharePointFolderAsync(SharePointExpandableObjcect.Title);
            folders = folders.OrderBy(s => s.Name);
            Log.LogMessage("Loaded folders in " + FullPath, 2);
            // Get files in the library. The library is specific by Title
            var files = await SharePointService.GetSharePointFilesAsync(SharePointExpandableObjcect.Title);
            files = files.OrderBy(s => s.Name);
            Log.LogMessage("Loaded files in " + FullPath, 2);
            folders.ToList().ForEach(folder =>
            {
                folder.ParentSharePointLibrary = SharePointExpandableObjcect as SharePointLibrary;
                //folder.RootUrl = folder.ParentSharePointLibrary.GetSharePointSiteRootUrl();
                //folder.Url = folder.RootUrl + folder.RelativeUrl;

                // folder.RootUrl = folder.ParentSharePointLibrary.RootUrl;
                // (SharePointExpandableObjcect as SharePointLibrary).SharePointLibraryItems.Add(folder);
                ((ISharePointFileContainer) SharePointExpandableObjcect).SharePointLibraryItems.Add(folder);
                // folder.RootUrl = SharePointExpandableObjcect.RootUrl;
                // use SharePointService from parent node
                var node = new SiteNodeTreeNode(folder.Name, 9, 9, SharePointService, repoProvider)
                {
                    HadFillDataFromRemote = false,
                    SiteId = SiteId,
                    SharePointExpandableObjcect = folder,
                    ContextMenuStrip = SiteItemMenuStrip,
                    SiteItemMenuStrip = SiteItemMenuStrip
                };
                node.Nodes.Add("Dump Node");
                Nodes.Add(node);
            });

            files.ToList().ForEach(file =>
            {
                file.ParentSharePointLibrary = SharePointExpandableObjcect as SharePointLibrary;
                //file.RootUrl = file.ParentSharePointLibrary.GetSharePointSiteRootUrl();
                //file.Url = file.RootUrl + file.RelativeUrl;
                //file.RootUrl = file.ParentSharePointLibrary.RootUrl;
                (((ISharePointFileContainer) SharePointExpandableObjcect).SharePointLibraryItems).Add(file);
            });
            HadFillDataFromRemote = true;
        }

        /// <summary>
        /// Call to expand the node if it is a site node
        /// </summary>
        private async Task ExpandSiteNodeAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(((SharePointSite) SharePointExpandableObjcect).User))
                {
                    Log.LogMessage("Get the current user", 2);
                    // get current user
                    var currentUser = await SharePointService.GetCurrentUserAsync();
                    ((SharePointSite) SharePointExpandableObjcect).User = currentUser;
                }

                var siteDetails = repoProvider.SiteRepository.GetById(SiteId);
                if (siteDetails == null)
                {
                    throw new Exception($"Site {SiteId} does not exist");
                }

                if (siteDetails.ToSiteDetails().IncludeSubSites)
                {
                    //get sites
                    var sites = await SharePointService.GetSubSharePointSitesAsync();
                    sites = sites.OrderBy(s => s.Title);
                    Log.LogMessage("Loaded SP sites in " + FullPath, 2);
                    sites.ToList().ForEach(site =>
                    {
                        //site.User = currentUser;
                        site.ParentSharePointSite = (SharePointSite)SharePointExpandableObjcect;
                        site.LocalId = site.ParentSharePointSite.LocalId;

                        //site.RootUrl = site.ParentSharePointSite.RootUrl;
                        ((SharePointSite)SharePointExpandableObjcect).SharePointSiteItems.Add(site);

                        var node = new SiteNodeTreeNode(site.Title, 2, 2,
                            SharepointServiceProvider.GetSharePointService(new SiteSoapClientMapper
                            {
                                Id = SharePointService.SiteSoapClientMapper.Id,
                                Url = site.Url,
                                RootUrl = SharePointService.SiteSoapClientMapper.RootUrl
                            }, SharePointService.SharePointServerVersion,
                                SharePointService.Credential), repoProvider)
                        {
                            HadFillDataFromRemote = false,
                            SiteId = SiteId,
                            SharePointExpandableObjcect = site,
                            ContextMenuStrip = SiteItemMenuStrip,
                            SiteItemMenuStrip = SiteItemMenuStrip
                        };
                        node.Nodes.Add("Dump Node");
                        Nodes.Add(node);
                    });
                }
                

                // Get libraries in the current site
                var libraries = await SharePointService.GetSharePointLibrariesAsync();
                libraries = libraries.OrderBy(s => s.Title);
                Log.LogMessage("Load libraries in " + FullPath, 2);
                libraries.ToList().ForEach(list =>
                {
                    list.ParentSharePointSite = (SharePointSite)SharePointExpandableObjcect;
                    //list.RootUrl = list.ParentSharePointSite.RootUrl;
                    //list.Url = list.ParentSharePointSite.RootUrl + list.RelativeUrl;

                    // Check the value is empty (not empty with sharepoitn 2010, 2013 and office 365)
                    //if (string.IsNullOrEmpty(list.RelativeUrl))
                    //{
                    //    list.RelativeUrl = SharePointService.GetSharePointLibrary(list.Title).RelativeUrl;
                    //}

                    ((SharePointSite) SharePointExpandableObjcect).SharePointSiteItems.Add(list);
                    // use SharePointService from parent node
                    var node = new SiteNodeTreeNode(list.Title, 8, 8, SharePointService, repoProvider)
                    {
                        HadFillDataFromRemote = false,
                        SiteId = SiteId,
                        SharePointExpandableObjcect = list,
                        ContextMenuStrip = SiteItemMenuStrip,
                        SiteItemMenuStrip = SiteItemMenuStrip
                    };

                    node.Nodes.Add("Dump Node");
                    Nodes.Add(node);
                });
                HadFillDataFromRemote = true;
                if (Level == 1)
                {
                    SelectedImageIndex = 1;
                    ImageIndex = 1;
                }
            }
            catch (Exception ex)
            {
                if (Level == 1)
                {
                    SelectedImageIndex = 4;
                    ImageIndex = 4;
                }
                else
                {
                    SelectedImageIndex = 10;
                    ImageIndex = 10;
                }
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.LogException(ex);
            }
        }

        #endregion
    }
}

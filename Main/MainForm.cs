using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Main.Core;
using Main.Libs;
using Main.Properties;
using Main.Repos;
using Main.Services;
using Main.Services.Domains;
using Main.UserControls;
using Services;
using Form = System.Windows.Forms.Form;
using System.ComponentModel;
using System.Globalization;
using Core.Libs;
using Entities;

namespace Main
{
    public partial class MainForm : Form
    {
        /*
                private readonly CompositionContainer container;
        */
        private readonly List<AsyncTask> _tasks = new List<AsyncTask>();
#pragma warning disable 169
        private readonly ErrorTracker _errorTracker;
#pragma warning restore 169
        private int _lastSortHistoryFileColumn;
        private SortOrder _lastSortHistoryFileListViewOrder = SortOrder.Descending;

        private int _lastSortFileListViewColumn;
        private SortOrder _lastSortFileListViewOrder = SortOrder.Ascending;
        private bool _fileSelected;
        private bool _fileVersionSelected;
        public FormAction FormAction { get; }
        private readonly IRepoProvider _repoProvider;

        //[Import(typeof(IAdditionalSPSite))]
        //public IAdditionalSPSite AdditionalSPSite { get; set; }
        public MainForm(IRepoProvider repoProvider, FormAction action = FormAction.None)
        {
            _repoProvider = repoProvider;
            InitializeComponent();

            //#if !DEBUG
            //            this.TopMost = true;
            //#endif

            fileListView.RepoProvider = repoProvider;
            sitesTreeView.Nodes[0].ContextMenuStrip = rootNodeContextMenuStrip;
            FormAction = action;
        }

        private async void Main_Load(object sender, EventArgs e)
        {
            // toolStripStatusLabel config
            toolStripStatusLabel.Margin = new Padding(0, 3, Width - 538, 2);

            // Treeview dont have any site nodes, need to add the site node to the treeview by call ReloadTreeView
            ReloadTreeView();

            rbSaveVersion.Visible = false;
            rbSaveNewFile.Visible = false;
            edtMe.Visible = false;

            if (FormAction == FormAction.GetUrl)
            {
                Text = @"SharePoint DMS - Load From DMS";
                btnOk.Text = @"Select";
            }
            if ((FormAction == FormAction.Upload) || (FormAction == FormAction.UploadVersion))
            {
                Text = @"SharePoint DMS - Upload - Select a Library or Folder";
                btnOk.Text = @"Upload";

                //
                rbSaveNewFile.Visible = true;
                rbSaveVersion.Visible = true;
                //
                rbSaveNewFile.Checked = (FormAction == FormAction.Upload);
                rbSaveVersion.Checked = (FormAction == FormAction.UploadVersion);
                rbSaveNewFile.Enabled = (FormAction == FormAction.Upload);
                rbSaveVersion.Enabled = FormAction == FormAction.UploadVersion;
                edtMe.Visible = true;
                edtMe.Enabled = FormAction == FormAction.Upload;
                //
                if (_repoProvider.UploadingFiles.Files.Count >= 1)
                {

                    var s = _repoProvider.UploadingFiles.Files[0].LocalPath ?? string.Empty;
                    if (s != string.Empty)
                    {
                        s = Path.GetFileName(s);
                    }
                    edtMe.Text = s;
                }
            }
            if (FormAction == FormAction.None)
            {
                btnOk.Text = @"Close";
            }

            if (Settings.Default.FormWindowStateMaximized)
            {
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                if (Settings.Default.Width != 0 && Settings.Default.Height != 0)
                {
                    Width = Settings.Default.Width;
                    Height = Settings.Default.Height;
                }
            }

            // Select node from last node
            var path = Settings.Default.LastSelectionPath;
            if (!string.IsNullOrWhiteSpace(path))
            {
                var pathList = path.Split('\\').Skip(1).ToList();
                if (pathList.Count > 0)
                {
                    var note = sitesTreeView.Nodes[0].Nodes.Cast<TreeNode>().FirstOrDefault(s => s.Text == pathList[0]);
                    if (note != null)
                    {

                        sitesTreeView.Enabled = false;
                        Log.LogMessage("Reselect SharePoint object node from last selection", 1);

                        await ExpandMyLitleBoysAsync(note as SiteNodeTreeNode, pathList);
                        sitesTreeView.Enabled = true;
                    }
                }
            }
        }

        private void ProcessEnd(Guid guid)
        {
            lock (this)
            {
                var task = _tasks.FirstOrDefault(s => s.Guid == guid);
                if (task != null)
                {
                    _tasks.Remove(task);
                }
                // asyncTaskCount--;
                if (!_tasks.Any())
                {
                    toolStripStatusLabel.Text = @"Ready";
                    toolStripProgressBar.Value = 100;
                    toolStripProgressBar.Value = 0;
                    toolStripProgressBar.Visible = false;
                    Cursor = Cursors.Default;
                }
                else
                {
                    var nextTask = _tasks[0];
                    toolStripStatusLabel.Text = nextTask.Text;
                }
            }
        }

        private void ProcessBegin(Guid guid, string processLabel = "Loading SharePoint objects ...")
        {
            lock (this)
            {
                var newTask = new AsyncTask()
                {
                    Guid = guid,
                    Text = processLabel
                };
                _tasks.Add(newTask);

                if (!_tasks.Any(s => s.IsProcessing))
                {
                    newTask.IsProcessing = true;
                    toolStripStatusLabel.Text = processLabel;
                    toolStripProgressBar.Visible = true;
                    toolStripProgressBar.Value = 70;
                    Cursor = Cursors.AppStarting;
                }
            }
        }

        private async Task ExpandMyLitleBoysAsync(SiteNodeTreeNode node, List<string> path)
        {
            Log.LogMessage("Auto expanded " + node.Text, 2);

            path.RemoveAt(0);
            var taskGuid = Guid.NewGuid();
            ProcessBegin(taskGuid, $"Loading {node.Text} node ...");

            // Select a first site node
            if (node.Level == 1)
            {
                // This make sure first level sharepoint site node contain relative url
                try
                {
                    await node.InitRootSharePointSiteFromRemoteAsync();
                }
                catch (Exception ex)
                {
                    DialogHelper.ShowErrorDialog(ex, Log.LogException(ex));
                    //var message = ex.InnerException == null ? $"{ex.Message}{Environment.NewLine}{ex.StackTrace}" :
                    //    $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}{Environment.NewLine}{ex.StackTrace}";
                    //MessageBox.Show(message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //Log.LogException(ex);
                }
            }

            await node.ExpandFromRemoteAsync();
            ProcessEnd(taskGuid);
            if (node.Nodes.Count > 0)
            {
                node.Nodes.RemoveAt(0);
            }
            node.Expand();
            if (path.Count == 0)
            {
                sitesTreeView.SelectedNode = node;
                return;
            }

            var node1 = node.Nodes.Cast<TreeNode>().FirstOrDefault(s => s.Text == path[0]);
            if (node1 != null)
            {
                await ExpandMyLitleBoysAsync(node1 as SiteNodeTreeNode, path);
            }
        }

        // Root context menu strip click add new site handler
        private void addSiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var addSiteDialog = new SiteDetailsForm(null, CrudModel.Add, null);
            if (addSiteDialog.ShowDialog() == DialogResult.OK)
            {
                var newSite = addSiteDialog.Model;
                _repoProvider.SiteRepository.Add(new Site()
                {
                    Credential = newSite.Credential.ToCredential(),
                    SharePointServerVersion = newSite.SharePointServerVersion,
                    Name = newSite.Name,
                    Enable = newSite.Enable,
                    Description = newSite.Description,
                    RequireAuthentication = newSite.RequireAuthentication,
                    Url = newSite.Url
                });

                // Save to site.xml
                _repoProvider.SaveChanges();

                ReloadTreeView();
            }
        }

        #region Helper methods

        /// <summary>
        /// Get the sites that are configured and stored in the database by users
        /// </summary>
        private void ReloadTreeView()
        {
            // Select the first node
            if (sitesTreeView.Nodes.Count > 0)
            {
                sitesTreeView.SelectedNode = sitesTreeView.Nodes[0];
                sitesTreeView.Nodes[0].Nodes.Clear();
            }

            // Get all sites from local database
            var sites = _repoProvider.SiteRepository.GetEnableSites().OrderBy(s => s.Name).ToList();

            sites.ToList().ForEach(s =>
            {
                var mapper = new SiteSoapClientMapper()
                {
                    Id = s.Id,
                    Url = s.Url,
                    RootUrl = s.RootUrl
                };

                var siteNode = new SiteNodeTreeNode
                    (s.Name, 1, 1,
                        SharepointServiceProvider.GetSharePointService(mapper, s.SharePointServerVersion,
                            s.Credential.ToClearTextCredential()), _repoProvider)
                {
                    ContextMenuStrip = siteContextMenuStrip,
                    SiteItemMenuStrip = siteNodeContextMenuStrip,
                    ToolTipText = s.Description,
                    SiteId = s.Id,
                    SharePointExpandableObjcect = new SharePointSite()
                    {
                        Url = s.Url,
                        Description = s.Description,
                        Title = string.Empty,
                        // RootUrl = s.RootUrl,
                        LocalId = s.Id
                    },
                };
                siteNode.Nodes.Add(new TreeNode("Mock up node"));
                sitesTreeView.Nodes[0].Nodes.Add(siteNode);
            });

            // Expand the top node
            if (!sitesTreeView.Nodes[0].IsExpanded)
            {
                sitesTreeView.Nodes[0].Expand();
            }
        }

        /// <summary>
        /// Select history version
        /// </summary>
        private void SelectHistoryVersionFile()
        {
            // Selected versions
            var selectediItems = fileVersionListView.SelectedItems.Cast<ListViewItem>().ToList();
            if (selectediItems.Any())
            {
                // Select the file
                _repoProvider.SelectedFiles.Result = new Result() { Status = ResultStatus.Pending };
                _repoProvider.SelectedFiles.Files = new List<SelectedFile>();
                _repoProvider.SaveChanges(ResultMode.SelectFiles);

                var fileVersion = (SharePointFileVersion) selectediItems.First().Tag;

                _repoProvider.SelectedFiles.Files.Add(new SelectedFile()
                {
                    LocalId = fileVersion.SharePointFile.GetSharePointSiteLocalId(),
                    UiVersionLabel = fileVersion.VersionLabel,
                    Author = fileVersion.CreatedBy,
                    Url = fileVersion.SharePointFile.RootUrl + fileVersion.SharePointFile.RelativeUrl,
                    RelativeUrl = fileVersion.SharePointFile.RelativeUrl,
                    Name = fileVersion.SharePointFile.Name,
                    SiteUrl = fileVersion.SharePointFile.GetSharePointSiteUrl(),
                    FileVersionUrl = fileVersion.Url,
                    IsCurrentVersion = fileVersion.SharePointFile.RootUrl + fileVersion.SharePointFile.RelativeUrl == fileVersion.Url,
                    TreeViewPath = $"{sitesTreeView.SelectedNode.FullPath}\\{fileVersion.SharePointFile.Name}"
                });
                _repoProvider.SelectedFiles.Result.Status = ResultStatus.Success;
                _repoProvider.SaveChanges(ResultMode.SelectFiles);
                MessageBox.Show(@"The file was selected successfully", @"Successfully", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

#if !DEBUG
                Close(); 
#endif
            }
        }
        #endregion

        private async void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedNode = (SiteNodeTreeNode)sitesTreeView.SelectedNode;
            var siteDetails = _repoProvider.SiteRepository.GetById(selectedNode.SiteId).ToSiteDetails();
            var siteEdit = new SiteDetailsForm(siteDetails, CrudModel.Edit,
                SharepointServiceProvider.GetSharePointService(new SiteSoapClientMapper()
                {
                    Id = siteDetails.Id,
                    Url = siteDetails.Url,
                    RootUrl = selectedNode.SharePointService.SiteSoapClientMapper.RootUrl
                }, siteDetails.SharePointServerVersion, siteDetails.Credential));

            if (siteEdit.ShowDialog() == DialogResult.OK)
            {
                siteDetails = siteEdit.Model;
                var site = _repoProvider.SiteRepository.GetById(siteDetails.Id);
                site = siteDetails.ToSite(site);
                _repoProvider.SaveChanges();

                // selectedNode.SiteDetails = siteDetails;
                selectedNode.ToolTipText = siteDetails.Description;
                selectedNode.SharePointExpandableObjcect = new SharePointSite()
                {
                    Url = siteDetails.Url,
                    Description = siteDetails.Description,
                    Title = string.Empty,
                    // RootUrl = selectedNode.SharePointService.SiteSoapClientMapper.RootUrl,
                    LocalId = siteDetails.Id
                };

                txtAdressUrl.Text = selectedNode.SharePointExpandableObjcect.GetSharePointSiteUrl();
                var mapper = new SiteSoapClientMapper()
                {
                    Id = site.Id,
                    Url = site.Url,
                    RootUrl = site.RootUrl
                };

                selectedNode.SharePointService =
                    SharepointServiceProvider.GetSharePointService(mapper, site.SharePointServerVersion, site.Credential.ToClearTextCredential());
                //sitesTreeView.Cursor = Cursors.WaitCursor;
                //refreshToolStripMenuItem1_Click(sender, e);
                //sitesTreeView.Cursor = Cursors.Default;

                fileListView.Items.Clear();
                selectedNode.Nodes.Clear();
                Cursor = Cursors.WaitCursor;
                // Get data from remote then fill to the node
                //var subNodes = expandingNodeData.
                await selectedNode.ExpandFromRemoteAsync();
                Cursor = Cursors.Default;
                // Set HadFillDataFromRemote to indicate that the tree node has tryed to load data from remote 
                selectedNode.HadFillDataFromRemote = true;
                selectedNode.Text = site.Name;
            }
        }

        /// <summary>
        /// Delete site node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedNode = (SiteNodeTreeNode)sitesTreeView.SelectedNode;
            var siteDetails = _repoProvider.SiteRepository.GetById(selectedNode.SiteId).ToSiteDetails();
            var siteDelete = new SiteDetailsForm(siteDetails, CrudModel.Delete,
                SharepointServiceProvider.GetSharePointService(new SiteSoapClientMapper()
                {
                    Url = siteDetails.Url,
                    Id = siteDetails.Id,
                    // RootUrl = siteDetails.Url
                }, siteDetails.SharePointServerVersion, siteDetails.Credential));
            if (siteDelete.ShowDialog() == DialogResult.OK)
            {
                _repoProvider.SiteRepository.Delete(siteDetails.Id);
                _repoProvider.SaveChanges();
                ReloadTreeView();
            }
        }

        #region siteTreeView event handlers

        /// <summary>
        /// Just Expand the node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void sitesTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // Get the node need to be expanded
            var expandingNode = e.Node as SiteNodeTreeNode;
            // await ProcessSiteTreeViewNode(expandingNode, true);
            // Check the node is not the top node of the tree view
            if (expandingNode != null && expandingNode.Level >= 1)
            {
                // Ignore if selectedNode is under loading
                if (expandingNode.IsLoading)
                {
                    return;
                }
                // Check the node has be filled before
                if (!expandingNode.HadFillDataFromRemote)
                {
                    var taskGuid = Guid.NewGuid();
                    // sitesTreeView.Cursor = Cursors.WaitCursor;
                    ProcessBegin(taskGuid, $"Loading {expandingNode.Text} node ...");
                    expandingNode.IsLoading = true;
                    expandingNode.Nodes.Clear();
                    // Select a first site node
                    if (expandingNode.Level == 1)
                    {
                        // This make sure first level sharepoint site node contain relative url
                        await expandingNode.InitRootSharePointSiteFromRemoteAsync();
                    }

                    // Get data from remote then fill to the node
                    await expandingNode.ExpandFromRemoteAsync();

                    ProcessEnd(taskGuid);
                    // sitesTreeView.Cursor = Cursors.Default;

                    // Set HadFillDataFromRemote to indicate that the tree node has tryed to load data from remote 
                    expandingNode.HadFillDataFromRemote = true;

                    // Check if the note not expanded, expand it
                    if (!expandingNode.IsExpanded)
                    {
                        expandingNode.Expand();
                    }

                    // Mark the note finished loading data
                    expandingNode.IsLoading = false;
                }
            }
        }

        /// <summary>
        /// Expand the node and get items of the node then fill it into the right list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void sitesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var selectedNode = e.Node as SiteNodeTreeNode;

            // 2_ Clear fileListView items
            fileListView.SelectedItems.Clear();
            fileListView.Items.Clear();

            // 1_Check need to load data
            if (selectedNode == null || selectedNode.Level < 1)
            {
                txtAdressUrl.Text = "";
                return;
            }

            Log.LogMessage($"User selected {selectedNode.FullPath}", 1);

            txtAdressUrl.Text = selectedNode.SharePointExpandableObjcect.Url;

            // 3_ Check to ignore if selectedNode is under loading
            if (selectedNode.IsLoading)
            {
                return;
            }
            // 4_ If not under loading and not yet fill data => Get data from remote
            if (!selectedNode.HadFillDataFromRemote)
            {
                Log.LogMessage(
                    $"Selected node had not filled data from remote, need to load {selectedNode.Text} note from remote",
                    1);

                var taskGuid = Guid.NewGuid();
                ProcessBegin(taskGuid, $"Loading {selectedNode.Text} node ...");
                selectedNode.IsLoading = true;

                selectedNode.Nodes.Clear();

                // Select a first site node
                if (selectedNode.Level == 1)
                {
                    // This make sure first level sharepoint site node contain relative url
                    await selectedNode.InitRootSharePointSiteFromRemoteAsync();
                }

                // Log.LogMessage(string.Format("Expand {0} from remote", selectedNode.Text), 2);
                await selectedNode.ExpandFromRemoteAsync();
                ProcessEnd(taskGuid);
                selectedNode.HadFillDataFromRemote = true;
            }

            // This is required because the code is using async/await. This make sure the app work properly
            // Check the current selected node of sitesTreeView is same with the current loading node
            if (selectedNode == sitesTreeView.SelectedNode)
            {
                RefreshFilesListView(selectedNode);
            }
            selectedNode.IsLoading = false;
        }


        /// <summary>
        /// Site item refresh. Refresh the current site tree node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void refreshToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var selectedNode = (SiteNodeTreeNode) sitesTreeView.SelectedNode;
            if (selectedNode == null || selectedNode.IsLoading)
            {
                return;
            }
            //this.Cursor = Cursors.WaitCursor;
            fileListView.SelectedItems.Clear();
            fileListView.Items.Clear();
            // get the tree node need to refresh

            var taskGuid = Guid.NewGuid();
            ProcessBegin(taskGuid, $"Loading {selectedNode.Text}node ...");

            selectedNode.IsLoading = true;

            if (selectedNode.Level == 1)
            {
                await selectedNode.InitRootSharePointSiteFromRemoteAsync();
            }

            // Refresh the tree node
            await selectedNode.RefreshTreeNodeAsync();
            ProcessEnd(taskGuid);

            // This make sure sitesTreeView.SelectedNode was not changed before it complete the job
            // update file list view
            if (selectedNode == sitesTreeView.SelectedNode)
            {
                RefreshFilesListView(selectedNode);
            }
            selectedNode.IsLoading = false;
            // this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Make sure update to current selected item by mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sitesTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            var selectedNode = sitesTreeView.GetNodeAt(e.X, e.Y);
            if (selectedNode != null)
            {
                sitesTreeView.SelectedNode = selectedNode;
            }
        }
        #endregion

        #region contextMenuStrip event handlers

        private void refreshRootToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReloadTreeView();
        }

        #endregion

        private async void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileListView.Items.Clear();
            var selectedNode = sitesTreeView.SelectedNode as SiteNodeTreeNode;
            if (selectedNode != null && selectedNode.Level == 1)
            {
                selectedNode.Nodes.Clear();

                // sitesTreeView.Cursor = Cursors.WaitCursor;
                // Get data from remote then fill to the node
                //var subNodes = expandingNodeData.
                var taskGuid = Guid.NewGuid();
                ProcessBegin(taskGuid, $"Loading {selectedNode.Text} node ...");
                selectedNode.IsLoading = true;
                // Select a first site node
                if (selectedNode.Level == 1)
                {
                    // This make sure first level sharepoint site node contain relative url
                    await selectedNode.InitRootSharePointSiteFromRemoteAsync();
                }
                await selectedNode.ExpandFromRemoteAsync();
                // sitesTreeView.Cursor = Cursors.Default;

                // Set HadFillDataFromRemote to indicate that the tree node has tryed to load data from remote 
                selectedNode.HadFillDataFromRemote = true;
                selectedNode.IsLoading = false;
                ProcessEnd(taskGuid);
            }
        }

        private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Not Implemented", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Upload file to sharepoint server in the application (not use with CMD)
        private async Task UploadAsync()
        {
            // Get SiteNodeTreeNode from ListView instance, this is a "Folder" the file will be uploaded to
            var folder = fileListView.SiteNodeTreeNode;
            if (folder == null)
            {
                MessageBox.Show(@"Please select a library of folder", @"Infomation", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                // Relative url of the "Folder" that will contain the uploading file
                var relativeUrl = folder.SharePointExpandableObjcect.RelativeUrl;
                var openFileDialog = new OpenFileDialog()
                {
                    Title = @"Upload",
                    CheckFileExists = true,
                    CheckPathExists = true
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var fileName = Path.GetFileName(openFileDialog.FileName);
                    // Get existing file on server by file name
                    var existFile =
                        ((ISharePointFileContainer) folder.SharePointExpandableObjcect).SharePointLibraryItems.FirstOrDefault(s => s is SharePointFile && ((SharePointFile) s).Name == fileName);

                    // Contain url of uploading file after upload successfully
                    string uploadedUrl;
                    // exist => upload the file with check out if need
                    if (existFile != null)
                    {
                        // check version settings, if versioning settings was disable, show warning the file will be overwrite
                        if (!((ISharePointFileContainer) folder.SharePointExpandableObjcect).GetAncestorSharePointLibrary().EnableVersioning)
                        {
                            if (MessageBox.Show(
                                $@"The library is not enable versioning. Do you want to overwrite file {fileName}?", @"SharePoint Save", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                            {
                                return;
                            }
                        }
                        var taskGuid = Guid.NewGuid();
                        ProcessBegin(taskGuid, "Uploading file ...");
                        // need check out required by list settings?
                        if (((ISharePointFileContainer) folder.SharePointExpandableObjcect).GetAncestorSharePointLibrary().RequireCheckout)
                        {
                            // The file need to check out?
                            if (((SharePointFile) existFile).CanCheckOut)
                            {
                                var checkOutResult =
                                    await folder.SharePointService.CheckOutFileAsync(existFile.RootUrl + existFile.RelativeUrl,
                                        false.ToString(), null);
                                if (!checkOutResult)
                                {
                                    ProcessEnd(taskGuid);
                                    MessageBox.Show(@"Check out is failure", @"Check out is failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    // if check out fail, stop uploading
                                    return;
                                }
                            }
                        }

                        var localFilePath = openFileDialog.FileName;
                        var fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        Text += @" - Uploading ...";
                        _repoProvider.UploadedFiles.Files.Clear();
                        _repoProvider.UploadedFiles.Result.Status = ResultStatus.Pending;
                        _repoProvider.SaveChanges();

                        // Upload file
                        var result = await folder.UploadFileAsync(fileStream, fileName, relativeUrl);
                        ProcessEnd(taskGuid);

                        // Check that need refresh tree node
                        if (folder == sitesTreeView.SelectedNode)
                        {
                            var taskGuid1 = Guid.NewGuid();
                            ProcessBegin(taskGuid1, "Refreshing list ...");

                            await folder.RefreshTreeNodeAsync();
                            // Need refresh ListView?
                            ProcessEnd(taskGuid1);
                        }


                        fileStream.Close();
                        Text = Text.Substring(0, Text.Length - " - Uploading ...".Length);

                        if (result.All(s => s.ErrorCode == ErrorCode.Success))
                        {
                            uploadedUrl = result.First(t => t.ErrorCode == ErrorCode.Success).Url;
                            _repoProvider.UploadedFiles.Files.Add(new UploadedFile()
                            {
                                Url = result.First(t => t.ErrorCode == ErrorCode.Success).Url,
                                LocalId = folder.SharePointExpandableObjcect.GetSharePointSiteLocalId(),
                                Uploaded = true,
                                LocalPath = localFilePath,
                                Name = fileName,
                                SiteUrl = folder.SharePointExpandableObjcect.GetSharePointSiteUrl()
                            });

                            // Upload successfully, => need to check in the file?
                            if (((ISharePointFileContainer) fileListView.SiteNodeTreeNode.SharePointExpandableObjcect).GetAncestorSharePointLibrary().RequireCheckout)
                            {
                                var taskGuid2 = Guid.NewGuid();
                                ProcessBegin(taskGuid2, "Checking in file ...");
                                await folder.CheckinFileAsync(uploadedUrl);
                                ProcessEnd(taskGuid2);
                            }
                            if (folder == sitesTreeView.SelectedNode)
                            {
                                var taskGuid3 = Guid.NewGuid();
                                ProcessBegin(taskGuid3, "Refreshing list ...");
                                await folder.RefreshTreeNodeAsync();
                                RefreshFilesListView(folder);
                                ProcessEnd(taskGuid3);
                            }
                            _repoProvider.UploadedFiles.Result.Status = ResultStatus.Success;
                            _repoProvider.SaveChanges(ResultMode.UploadFiles);
                            RefreshFilesListView(sitesTreeView.SelectedNode as SiteNodeTreeNode);
                            // Not need - MessageBox.Show(@"Success", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Upload successfully, close the form
#if !DEBUG
                            Close(); 
#endif
                        }
                        else
                        {
                            _repoProvider.UploadedFiles.Result.Status = ResultStatus.Error;
                            _repoProvider.SaveChanges(ResultMode.UploadFiles);
                            var temp = result.First(s => s.ErrorCode != ErrorCode.Success);
                            MessageBox.Show(temp.Message, temp.ErrorCode.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            // Not close the form, allow user can retry
                        }
                    }
                    // Not exist => upload the file with out check out
                    else
                    {
                        var localFilePath = openFileDialog.FileName;
                        var fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        Text += @" - Uploading ...";
                        _repoProvider.UploadedFiles.Files.Clear();
                        _repoProvider.UploadedFiles.Result.Status = ResultStatus.Pending;
                        _repoProvider.SaveChanges(ResultMode.UploadFiles);
                        var taskGuid4 = Guid.NewGuid();
                        ProcessBegin(taskGuid4, "Uploading file ...");
                        var result = await folder.UploadFileAsync(fileStream, fileName, relativeUrl);
                        ProcessEnd(taskGuid4);
                        fileStream.Close();
                        Text = Text.Substring(0, Text.Length - " - Uploading ...".Length);
                        if (result.All(s => s.ErrorCode == ErrorCode.Success))
                        {
                            _repoProvider.UploadedFiles.Files.Add(new UploadedFile()
                            {
                                Url = result.First(t => t.ErrorCode == ErrorCode.Success).Url,
                                LocalId = folder.SharePointExpandableObjcect.GetSharePointSiteLocalId(),
                                Uploaded = true,
                                LocalPath = localFilePath,
                                Name = fileName,
                                SiteUrl = folder.SharePointExpandableObjcect.GetSharePointSiteUrl()
                            });
                            uploadedUrl = result.First(t => t.ErrorCode == ErrorCode.Success).Url;
                            // Upload successfully, => need to check in the file?
                            if (((ISharePointFileContainer) fileListView.SiteNodeTreeNode.SharePointExpandableObjcect).GetAncestorSharePointLibrary().RequireCheckout)
                            {
                                var taskGuid5 = Guid.NewGuid();
                                ProcessBegin(taskGuid5, "Checking in file ...");
                                await folder.CheckinFileAsync(uploadedUrl);
                                ProcessEnd(taskGuid5);
                            }
                            if (folder == sitesTreeView.SelectedNode)
                            {
                                var taskGuid6 = Guid.NewGuid();
                                ProcessBegin(taskGuid6, "Refreshing list ...");
                                await folder.RefreshTreeNodeAsync();
                                RefreshFilesListView(folder);
                                ProcessEnd(taskGuid6);
                            }
                            //repoProvider.UploadedFiles.Result.Status = ResultStatus.Success;
                            _repoProvider.UploadedFiles.Result.Status = ResultStatus.Success;
                            _repoProvider.SaveChanges();
                            // RefreshFilesListView(sitesTreeView.SelectedNode as SiteNodeTreeNode);
                            //Not needed - MessageBox.Show(@"Success", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
#if !DEBUG
                            Close(); 
#endif
                        }
                        else
                        {
                            _repoProvider.UploadedFiles.Result.Status = ResultStatus.Error;
                            _repoProvider.SaveChanges(ResultMode.UploadFiles);
                            var temp = result.First(s => s.ErrorCode != ErrorCode.Success);
                            MessageBox.Show(temp.Message, temp.ErrorCode.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// New File toolstrip click event handler (Upload file)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await UploadAsync();
        }

        /// <summary>
        /// DoubleClick to ListViewItem to download the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listItemslistView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                var seletecdItems = fileListView.SelectedItems.Cast<FilesListViewItem>().ToList();
                if (seletecdItems.Count == 1)
                {
                    var selectedItem = seletecdItems[0];
                    if (selectedItem.SharePointLibraryItem is SharePointFolder)
                    {
                        var node =
                            (fileListView).SiteNodeTreeNode.Nodes.Cast<SiteNodeTreeNode>()
                                .First(s => s.Text == selectedItem.SharePointLibraryItem.Name);
                        node.TreeView.SelectedNode = node;
                        node.Expand();
                        return;
                    }
                    if (selectedItem.SharePointLibraryItem is SharePointFile)
                    {
                        #region select selected file
                        if (FormAction == FormAction.GetUrl)
                        {
                            _repoProvider.SelectedFiles = new SelectedFiles()
                            {
                                Result = new Result() { Status = ResultStatus.Success },
                                Files = new List<SelectedFile>()
                            {
                                new SelectedFile()
                                {
                                    Name = selectedItem.SharePointLibraryItem.Name,
                                    Author = selectedItem.SharePointLibraryItem.Author,
                                    Url =
                                        selectedItem.SharePointLibraryItem.RootUrl +
                                        selectedItem.SharePointLibraryItem.RelativeUrl,
                                        RelativeUrl = selectedItem.SharePointLibraryItem.RelativeUrl,
                                    LocalId = selectedItem.SharePointLibraryItem.GetSharePointSiteLocalId(),
                                    SiteUrl = selectedItem.SharePointLibraryItem.GetSharePointSiteUrl(),
                                    IsCurrentVersion = true,
                                    UiVersionLabel =
                                        ((SharePointFile) selectedItem.SharePointLibraryItem).UIVersionLabel,
                                    TreeViewPath =
                                        $"{sitesTreeView.SelectedNode.FullPath}\\{selectedItem.SharePointLibraryItem.Name}"
                                }
                            }.ToList()

                            };
                            _repoProvider.SaveChanges(ResultMode.SelectFiles);
                            Close();
                            return;
                        }
                        #endregion
                        return;
                    }

                    MessageBox.Show(@"Error", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowErrorDialog(ex, Log.LogException(ex));
                //var message = ex.InnerException == null ? $"{ex.Message}{Environment.NewLine}{ex.StackTrace}" :
                //        $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}{Environment.NewLine}{ex.StackTrace}";
                //MessageBox.Show(message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Log.LogException(ex);
            }
        }

        private void listItemslistView_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    var item = fileListView.GetItemAt(e.X, e.Y);
                    if (item != null)
                    {
                        var fileListItem = (FilesListViewItem) item;
                        // check out menuitem
                        oneFileWasSelectedContextMenuStrip.Items[6].Enabled = fileListItem.CanCheckOut;
                        // check in menuitem
                        oneFileWasSelectedContextMenuStrip.Items[7].Enabled = fileListItem.CanCheckIn;
                        oneFileWasSelectedContextMenuStrip.Show(fileListView, e.Location);
                    }
                    else
                    {
                        if ((FormAction == FormAction.Upload) || (FormAction == FormAction.UploadVersion))
                        {
                            return;
                        }
                        // if have no files was selected, display noFilesWasSelectedContextMenuStrip
                        noFilesWasSelectedContextMenuStrip.Show(fileListView, e.Location);
                    }
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowErrorDialog(ex, Log.LogException(ex));
                //var message = ex.InnerException == null ? $"{ex.Message}{Environment.NewLine}{ex.StackTrace}" :
                //        $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}{Environment.NewLine}{ex.StackTrace}";
                //MessageBox.Show(message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Log.LogException(ex);
            }
            // display context menu
            
        }

        /// <summary>
        /// Download one file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get file as a list view item
            // var source = ((sender as ToolStripMenuItem).Owner as ContextMenuStrip).SourceControl as FilesListView;
            if (fileListView.SelectedItems.Count > 1)
            {
                MessageBox.Show(@"Error", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var listItem = fileListView.SelectedItems[0] as FilesListViewItem;
            if (listItem == null)
            {
                return;
            }
            Text += @" - Downloading ...";

            _repoProvider.DownloadedFiles.Result.Status = ResultStatus.Pending;
            _repoProvider.DownloadedFiles.Files.Clear();
            _repoProvider.SaveChanges(ResultMode.DownloadFiles);

            if (string.IsNullOrWhiteSpace(_repoProvider.DownloadDirectory))
            {
                var dialog = (new FolderBrowserDialog());
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _repoProvider.DownloadDirectory = dialog.SelectedPath;
                    _repoProvider.SaveChanges();
                }
                else
                {
                    return;
                }
            }
            //if (!Directory.Exists(repoProvider.DownloadDirectory))
            //{
            //    Directory.CreateDirectory(repoProvider.DownloadDirectory);
            //}

            //if (string.IsNullOrWhiteSpace(repoProvider.DownloadDirectory))
            //{
            //    repoProvider.DownloadedFiles.Result.Status = ResultStatus.Error;
            //    repoProvider.SaveChanges(ResultMode.DownloadFiles);
            //    MessageBox.Show(Constants.MissDownloadDirectory, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            try
            {
                if (!Directory.Exists(_repoProvider.DownloadDirectory))
                {
                    Directory.CreateDirectory(_repoProvider.DownloadDirectory);
                }
            }
            catch (Exception ex)
            {
                _repoProvider.DownloadedFiles.Result.Status = ResultStatus.Error;
                _repoProvider.SaveChanges(ResultMode.DownloadFiles);
                DialogHelper.ShowErrorDialog(ex, Log.LogException(ex));
                //var message = ex.InnerException == null ? $"{ex.Message}{Environment.NewLine}{ex.StackTrace}" :
                //        $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}{Environment.NewLine}{ex.StackTrace}";
                //MessageBox.Show(message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Log.LogException(ex);
                return;
            }

            var taskGuid = Guid.NewGuid();
            try
            {
                ProcessBegin(taskGuid, "Downloading file ...");
                await listItem.DownloadFileAsync(_repoProvider.DownloadDirectory);
                MessageBox.Show(@"Download " + listItem.SharePointLibraryItem.Name + @" successfully", @"Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                _repoProvider.DownloadedFiles.Result.Status = ResultStatus.Success;
                _repoProvider.SaveChanges(ResultMode.DownloadFiles);
            }
            catch (Exception ex)
            {
                _repoProvider.DownloadedFiles.Result.Status = ResultStatus.Error;
                _repoProvider.SaveChanges(ResultMode.DownloadFiles);

                DialogHelper.ShowErrorDialog(ex, Log.LogException(ex));
                //var message = ex.InnerException == null ? $"{ex.Message}{Environment.NewLine}{ex.StackTrace}" :
                //        $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}{Environment.NewLine}{ex.StackTrace}";
                //MessageBox.Show(message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Log.LogException(ex);
            }
            finally
            {
                ProcessEnd(taskGuid);
                Text = Text.Substring(0, Text.Length - " - Downloading ...".Length);
            }
        }

        /// <summary>
        /// Copy Url context menu item click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyUrlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var source = (FilesListView) ((ContextMenuStrip) ((ToolStripMenuItem) sender).Owner).SourceControl;
            if (source.SelectedItems.Count > 1)
            {
                MessageBox.Show(@"Error", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var listItem = (FilesListViewItem) source.SelectedItems[0];
            Clipboard.SetText(listItem.SharePointLibraryItem.RootUrl + listItem.SharePointLibraryItem.RelativeUrl);
        }

        /// <summary>
        /// Get the items and display them in the listview
        /// </summary>
        /// <param name="node"></param>
        private void RefreshFilesListView(SiteNodeTreeNode node)
        {
            if (node == null) return;
            // Handle ListView
            if (node.TreeNodeType == TreeNodeType.Folder || node.TreeNodeType == TreeNodeType.Library)
            {
                fileListView.SiteNodeTreeNode = node;
                fileListView.SharePointService = node.SharePointService;
                fileListView.SharePointFileContainer = node.SharePointExpandableObjcect as ISharePointFileContainer;
                fileListView.PopulateFilesIntoFilesListView();
                fileVersionListView.SharePointService = node.SharePointService;
            }
            if (node.TreeNodeType == TreeNodeType.Site)
            {
                fileListView.SiteNodeTreeNode = null;
                fileListView.SharePointFileContainer = null;
                fileListView.PopulateFilesIntoFilesListView();
            }
            fileVersionListView.Items.Clear();
        }

        ///// <summary>
        ///// Refresh file version list
        ///// </summary>
        ///// <param name="fileUrl">Absolute url</param>
        ///// <param name="node">Node instance</param>
        //private void RefreshFileVersionListView(string fileUrl, SiteNodeTreeNode node)
        //{
        //    node.SharePointService.GetVersions(fileUrl);
        //}

        private void siteManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var siteManager = new SitesForm(_repoProvider);
            if (siteManager.ShowDialog() == DialogResult.OK)
            {
                ReloadTreeView();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnOk_Click(object sender, EventArgs e)
        {
            //// Save last selection path
            //var lastSelection = sitesTreeView.SelectedNode.FullPath;
            //Settings.Default.LastSelectionPath = lastSelection;
            //Settings.Default.Save();
            // GetUrl: call by command-line
            // 1) save the urls of the selected files
            // None: run Main.exe directly
            if (FormAction == FormAction.None)
            {
                Close();
            }
            if (FormAction == FormAction.GetUrl)
            {
                SelectFilesHandler();
            }

            if (FormAction == FormAction.UploadVersion)
            {
                // This block never be called
                await UploadFileVersionHandlerAsync();
            }

            if (FormAction == FormAction.Upload)
            {
                // Log.LogMessage("Upload file as new profile");
                await UploadNewFileHandlerAsync();
            }
        }

        /// <summary>
        /// Upload new profile
        /// </summary>
        private async Task UploadNewFileHandlerAsync()
        {
            // Get site note from fileListView control
            var siteNode = fileListView.SiteNodeTreeNode;

            // Check selected node is a folder or a library
            if (siteNode != null &&
                (siteNode.SharePointExpandableObjcect is SharePointFolder ||
                 siteNode.SharePointExpandableObjcect is SharePointLibrary))
            {
                // Get uploading file that was set in registry
                var uploadingFiles = _repoProvider.UploadingFiles;
                if (!uploadingFiles.Files.Any())
                {
                    MessageBox.Show(@"There are no files to upload", @"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate uploading file name
                var validationResult = ValidateFileName(edtMe.Text);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    MessageBox.Show(validationResult, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    edtMe.Focus();
                    return;
                }

                var fileName = edtMe.Text;

                // Check file name that with out extension
                if (Path.GetFileNameWithoutExtension(fileName) == fileName)
                {
                    if (MessageBox.Show(@"Do you want to save the file without file extension?", @"Warning",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        edtMe.Focus();
                        return;
                    }
                }

                // In this release, upload the first file only
                var item = uploadingFiles.Files.First().LocalPath;
                try
                {
                    var fileStream = new FileStream(item, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    // upload the file in the list
                    // 1) check exist file?
                    var existFile =
                        ((ISharePointFileContainer)siteNode.SharePointExpandableObjcect)
                            .SharePointLibraryItems.FirstOrDefault(
                                s => s is SharePointFile && ((SharePointFile)s).Name == fileName);

                    // If file is existing, display warning message
                    if (existFile != null)
                    {
                        MessageBox.Show(@"The file was existed. Please use a new name or select upload file as a new version.",
                            @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Clear uploaded files from previous session
                    _repoProvider.UploadedFiles.Files.Clear();
                    // Set upload process is Pending
                    _repoProvider.UploadedFiles.Result.Status = ResultStatus.Pending;
                    // Save changes
                    _repoProvider.SaveChanges(ResultMode.UploadFiles);

                    Text = @"Upload - Uploading files ...";

                    // Upload file
                    var result = await siteNode.UploadFileAsync(fileStream, fileName, siteNode.SharePointExpandableObjcect.RelativeUrl);
                    Log.LogMessage("Upload file as new profile", 1);
                    Log.LogMessage("Local file: " + item, 2);
                    Log.LogMessage("Remote file: " + siteNode.SharePointExpandableObjcect.RelativeUrl.CombineUrl(fileName), 2);
                    fileStream.Close();

                    if (result.All(t => t.ErrorCode == ErrorCode.Success))
                    {
                        var uploadedUrl = result.First(t => t.ErrorCode == ErrorCode.Success).Url;
                        // add current file upload result to the data.xml
                        _repoProvider.UploadedFiles.Files.Add(new UploadedFile()
                        {
                            LocalId = siteNode.SharePointExpandableObjcect.GetSharePointSiteLocalId(),
                            Url = result.First().Url,
                            Name = edtMe.Text,
                            LocalPath = item,
                            SiteUrl = siteNode.SharePointExpandableObjcect.GetSharePointSiteUrl(),
                            Uploaded = true
                        });

                        // Upload successfully, => need to check in the file?
                        if (((ISharePointFileContainer) fileListView.SiteNodeTreeNode.SharePointExpandableObjcect).GetAncestorSharePointLibrary().RequireCheckout)
                        {
                            await siteNode.CheckinFileAsync(uploadedUrl);
                        }
                        // siteNode.RefreshTreeNode();

                        _repoProvider.UploadedFiles.Result.Status = ResultStatus.Success;
                        _repoProvider.SaveChanges(ResultMode.UploadFiles);

                        await siteNode.RefreshTreeNodeAsync();
                        // Refresh file list view
                        RefreshFilesListView(siteNode);
                        MessageBox.Show(@"Upload file successfully", @"Success", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        Text = @"Upload - Select a Library or Folder";
                        // close the dialog if upload file successfully
                        Close();
                    }
                    else
                    {
                        _repoProvider.UploadedFiles.Result.Status = ResultStatus.Error;
                        _repoProvider.UploadedFiles.Result.Message = result.First().Message;
                        _repoProvider.SaveChanges(ResultMode.UploadFiles);
                        // Refresh tree node
                        await siteNode.RefreshTreeNodeAsync();
                        // Refresh file list view
                        RefreshFilesListView(siteNode);
                        MessageBox.Show(result.First().Message, @"Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        Text = @"Upload - Select a Library or Folder";

                        // not close the dialog if not success
                        // Close();
                    }
                }
                catch (Exception ex)
                {
                    _repoProvider.UploadedFiles.Result.Status = ResultStatus.Error;
                    _repoProvider.UploadedFiles.Result.Message = ex.Message;
                    _repoProvider.SaveChanges(ResultMode.UploadFiles);
                    //// Refresh tree node
                    //await siteNode.RefreshTreeNodeAsync();
                    //// Refresh file list view
                    //RefreshFilesListView(siteNode);

                    DialogHelper.ShowErrorDialog(ex, Log.LogException(ex));

                    //var message = ex.InnerException == null ? $"{ex.Message}{Environment.NewLine}{ex.StackTrace}" :
                    //    $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}{Environment.NewLine}{ex.StackTrace}";
                    //MessageBox.Show(message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //Log.LogException(ex);
                    Text = @"Upload - Select a Library or Folder";
                    // not close the dialog if not success
                    //Close();
                }
            }
        }

        /// <summary>
        /// // Upload new version
        /// </summary>
        /// <returns></returns>
        private async Task UploadFileVersionHandlerAsync()
        {
            var siteNode = fileListView.SiteNodeTreeNode;
            // Check did user select a Library or Folder
            if (siteNode == null ||
                (!(siteNode.SharePointExpandableObjcect is SharePointFolder) &&
                 !(siteNode.SharePointExpandableObjcect is SharePointLibrary)))
            {
                if (MessageBox.Show(@"There are no SharePoint Library or Folder is selected", "",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    Close();
                }
            }
            else
            {
                // get uploading files
                var uploadingFiles = _repoProvider.UploadingFiles;
                if (!uploadingFiles.Files.Any())
                {
                    MessageBox.Show(@"There are no files to upload", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Text = @"Upload - Uploading files ...";
                var item = uploadingFiles.Files.First().LocalPath;
                try
                {
                    // Get stream from local file
                    var fileStream = new FileStream(item, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    var fileName = Path.GetFileName(item);

                    // Check user selected a file from the file list, the app will upload the file as a new version of replaceFile
                    var replaceFile = fileListView.SelectedItems.Cast<FilesListViewItem>()
                                .Select(s => s.SharePointLibraryItem as SharePointFile)
                                .FirstOrDefault();

                    // If user did not select a file in the file list, the app will search a file that is same name with uploading file
                    var existFile = replaceFile ?? ((ISharePointFileContainer)siteNode.SharePointExpandableObjcect)
                        .SharePointLibraryItems.FirstOrDefault(
                            s => s is SharePointFile && ((SharePointFile)s).Name == fileName);

                    // Mean: user has selected a file in the file list or uploading file has existed in the file list
                    if (existFile != null)
                    {
                        // check version settings of the library
                        if (!((ISharePointFileContainer)siteNode.SharePointExpandableObjcect)
                                .GetAncestorSharePointLibrary().EnableVersioning)
                        {
                            if (MessageBox.Show(
                                $@"The library is not enable version settings. Do you want to overwrite file {fileName
                                    }?", @"SharePoint Save", MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Information) == DialogResult.No)
                            {
                                // Ignore uploading
                                return;
                            }
                        }
                        // need check out required by list settings?
                        if (((ISharePointFileContainer)siteNode.SharePointExpandableObjcect).GetAncestorSharePointLibrary().RequireCheckout)
                        {
                            // The file need to check out?
                            if (((SharePointFile)existFile).CanCheckOut)
                            {
                                var checkOutResult =
                                    siteNode.SharePointService.CheckOutFile(existFile.RootUrl + existFile.RelativeUrl,
                                        false.ToString(), null);
                                if (!checkOutResult)
                                {
                                    MessageBox.Show(@"Check out is failure", @"Check out is failure",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    // Ignore uploading
                                    return;
                                }
                            }
                        }

                        //
                        _repoProvider.UploadedFiles.Files.Clear();
                        _repoProvider.UploadedFiles.Result.Status = ResultStatus.Pending;
                        _repoProvider.SaveChanges(ResultMode.UploadFiles);

                        // upload the file to the server
                        var result = await siteNode.UploadFileAsync(fileStream, existFile.Name,
                            siteNode.SharePointExpandableObjcect.RelativeUrl);
                        fileStream.Close();
                        // siteNode.RefreshTreeNode();
                        if (result.All(t => t.ErrorCode == ErrorCode.Success))
                        {
                            var uploadedUrl = result.First(t => t.ErrorCode == ErrorCode.Success).Url;
                            // add current file upload result to the data.xml
                            _repoProvider.UploadedFiles.Files.Add(new UploadedFile()
                            {
                                LocalId = siteNode.SharePointExpandableObjcect.GetSharePointSiteLocalId(),
                                Url = result.First().Url,
                                Name = existFile.Name,
                                LocalPath = item,
                                SiteUrl = siteNode.SharePointExpandableObjcect.GetSharePointSiteUrl(),
                                Uploaded = true
                            });
                            // Upload successfully, => need to check in the file?
                            if (((ISharePointFileContainer) fileListView.SiteNodeTreeNode.SharePointExpandableObjcect)
                                    .GetAncestorSharePointLibrary().RequireCheckout)
                            {
                                await siteNode.CheckinFileAsync(uploadedUrl);
                            }

                            // The file was uploaded successfully
                            _repoProvider.UploadedFiles.Result = new Result() { Status = ResultStatus.Success };
                            _repoProvider.SaveChanges(ResultMode.UploadFiles);
                            // Refresh tree node
                            await siteNode.RefreshTreeNodeAsync();
                            // Refresh file list view
                            RefreshFilesListView(siteNode);
                            Text = @"Upload - Select a Library or Folder";
                            MessageBox.Show(@"Success", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                        else
                        {
                            // stop function if there is an error, save upload result
                            _repoProvider.UploadedFiles.Result.Status = ResultStatus.Error;
                            _repoProvider.UploadedFiles.Result.Message = result.First().Message;
                            _repoProvider.SaveChanges(ResultMode.UploadFiles);
                            // Refresh tree node
                            await siteNode.RefreshTreeNodeAsync();
                            // Refresh file list view
                            RefreshFilesListView(siteNode);
                            MessageBox.Show(result.First().Message, @"Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            Text = @"Upload - Select a Library or Folder";
                        }
                    }
                    else
                    {
                        // not exist => upload the file
                        var result = await siteNode.UploadFileAsync(fileStream, Path.GetFileName(item),
                            siteNode.SharePointExpandableObjcect.RelativeUrl);
                        fileStream.Close();
                        // siteNode.RefreshTreeNode();
                        if (result.All(t => t.ErrorCode == ErrorCode.Success))
                        {
                            var uploadedUrl = result.First(t => t.ErrorCode == ErrorCode.Success).Url;
                            // add current file upload result to the data.xml
                            _repoProvider.UploadedFiles.Files.Add(new UploadedFile()
                            {
                                LocalId = siteNode.SharePointExpandableObjcect.GetSharePointSiteLocalId(),
                                Url = result.First().Url,
                                Name = Path.GetFileName(item),
                                LocalPath = item,
                                SiteUrl = siteNode.SharePointExpandableObjcect.GetSharePointSiteUrl(),
                                Uploaded = true
                            });

                            // Upload successfully, => need to check in the file?
                            if (((ISharePointFileContainer) fileListView.SiteNodeTreeNode.SharePointExpandableObjcect)
                                    .GetAncestorSharePointLibrary().RequireCheckout)
                            {
                                await siteNode.CheckinFileAsync(uploadedUrl);
                            }

                            // The file was uploaded successfully
                            _repoProvider.UploadedFiles.Result = new Result() { Status = ResultStatus.Success };
                            _repoProvider.SaveChanges(ResultMode.UploadFiles);
                            // Refresh tree node
                            await siteNode.RefreshTreeNodeAsync();
                            // Refresh file list view
                            RefreshFilesListView(siteNode);
                            Text = @"Upload - Select a Library or Folder";
                            MessageBox.Show(@"Success", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            // stop foreach if there is an error, save upload result
                            _repoProvider.UploadedFiles.Result.Status = ResultStatus.Error;
                            _repoProvider.UploadedFiles.Result.Message = result.First().Message;
                            _repoProvider.SaveChanges(ResultMode.UploadFiles);
                            // Refresh tree node
                            await siteNode.RefreshTreeNodeAsync();
                            // Refresh file list view
                            RefreshFilesListView(siteNode);
                            MessageBox.Show(result.First().Message, @"Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            Text = @"Upload - Select a Library or Folder";
                        }
                    }
                }
                catch (Exception ex)
                {
                    // stop function if there is an error, save upload result
                    _repoProvider.UploadedFiles.Result.Status = ResultStatus.Error;
                    _repoProvider.UploadedFiles.Result.Message = ex.Message;
                    _repoProvider.SaveChanges(ResultMode.UploadFiles);
                    //// Refresh tree node
                    //await siteNode.RefreshTreeNodeAsync();
                    //// Refresh file list view
                    //RefreshFilesListView(siteNode);

                    DialogHelper.ShowErrorDialog(ex, Log.LogException(ex));

                    //var message = ex.InnerException == null ? $"{ex.Message}{Environment.NewLine}{ex.StackTrace}" :
                    //    $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}{Environment.NewLine}{ex.StackTrace}";
                    //MessageBox.Show(message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //Log.LogException(ex);
                    Text = @"Upload - Select a Library or Folder";
                }
            }
        }

        public void UploadFileVersionHandlerv2()
        {
            _repoProvider.UploadedFiles.Result.Status = ResultStatus.Pending;

            var uploadingFiles = _repoProvider.UploadingFiles;
            if (!uploadingFiles.Files.Any())
            {
                MessageBox.Show(@"There are no files to upload", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.LogMessage(@"There are no files to upload");
                _repoProvider.SaveChanges(ResultMode.UploadFiles);
                return;
            }
            if (uploadingFiles.Files.First().VersionHolderSelectedFileFile == null)
            {
                MessageBox.Show(@"Have not yet specified a file on server", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.LogMessage(@"Have not yet specified a file on server");
                _repoProvider.SaveChanges(ResultMode.UploadFiles);
                return;
            }

            var localFile = uploadingFiles.Files.First().LocalPath;
            if (!File.Exists(localFile))
            {
                Log.LogException(new FileNotFoundException(localFile));
                _repoProvider.UploadedFiles.Result.Status = ResultStatus.Error;
                MessageBox.Show(@"File not found", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _repoProvider.SaveChanges(ResultMode.UploadFiles);
                return;
            }
            // Log.LogMessage(string.Format("LocalPath: {0}", localFile));

            var remoteFile = uploadingFiles.Files.First().VersionHolderSelectedFileFile;

            Log.LogMessage($"Upload file as new version, {localFile} - {(remoteFile.IsCurrentVersion ? remoteFile.Url : remoteFile.FileVersionUrl)}");

            // Get SharePointService from remote file
            var site = _repoProvider.SiteRepository.GetByIdRecursive(remoteFile.LocalId);

            if (site == null)
            {
                MessageBox.Show(@"Cant specific a server to upload to", @"Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Log.LogException(new Exception("Can't specific the destination server."));
                _repoProvider.SaveChanges(ResultMode.UploadFiles);
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
                _repoProvider.UploadedFiles.Result.Status = ResultStatus.Error;
                _repoProvider.SaveChanges(ResultMode.UploadFiles);
                return;
            }

            if (site.IncludeSubSites)
            {
                site.SubSites = sharePointService.GetSubSharePointSites().Select(s => new Site()
                {
                    RootUrl = site.Url,
                    Url = $"{site.Url.TrimEnd('/')}/{s.RelativeUrl.TrimStart('/')}",
                    Enable = true,
                    SharePointServerVersion = site.SharePointServerVersion,
                    Credential = site.Credential,
                    RequireAuthentication = site.RequireAuthentication,
                }).ToList();
                var maxSubsite = site.SubSites.Where(s => remoteFile.Url.Contains(s.Url)).OrderByDescending(s => s.Url);
                if (maxSubsite.Any())
                {
                    if (maxSubsite.First().Url.Length > site.Url.Length)
                    {
                        site = maxSubsite.First();

                        sharePointService =
                            SharepointServiceProvider.GetSharePointService(new SiteSoapClientMapper()
                            {
                                Id = site.Id,
                                RootUrl = site.RootUrl,
                                Url = site.Url
                            }, site.SharePointServerVersion, site.Credential.ToClearTextCredential());
                    }
                }
            }

            // get site that is parent of remote file



            try
            {
                var sharePointFile = sharePointService.GetSharePointFile(remoteFile.Url);
                // The file was exist
                if (sharePointFile != null)
                {
                    // Get stream from local file
                    var fileStream = new FileStream(localFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    var parentList = sharePointFile.ParentSharePointLibrary ??
                                     sharePointService.GetParentSharePointLibraryByFileUrl(remoteFile.Url);


                    // check version settings of the library
                    if (!parentList.EnableVersioning)
                    {
                        if (MessageBox.Show($@"The library is not enable version settings. Do you want to overwrite file {
                                remoteFile.Name}?", @"SharePoint Save", MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information) == DialogResult.No)
                        {
                            _repoProvider.SaveChanges(ResultMode.UploadFiles);
                            // Ignore uploading
                            return;
                        }
                    }

                    // Check to need to check out?
                    // var fileName = Path.GetFileName(localFile);
                    if (parentList.RequireCheckout)
                    {
                        if (sharePointFile.CanCheckOut)
                        {
                            var checkOutResult = sharePointService.CheckOutFile(remoteFile.Url, false.ToString(), null);
                            if (checkOutResult)
                            {
                                _repoProvider.UploadedFiles.Files.Clear();
                                _repoProvider.UploadedFiles.Result.Status = ResultStatus.Pending;
                                _repoProvider.SaveChanges(ResultMode.UploadFiles);

                                var uploadResult = sharePointService.UploadFile(remoteFile.Url, fileStream);
                                Log.LogMessage("Upload file as new version", 1);
                                Log.LogMessage("Local file: " + localFile, 2);
                                Log.LogMessage("Remote file: " + remoteFile, 2);
                                // Upload successfully
                                if (uploadResult.FirstOrDefault(s => s.ErrorCode == ErrorCode.Success) != null)
                                {
                                    _repoProvider.UploadedFiles.Result = new Result() { Status = ResultStatus.Success };
                                    _repoProvider.SaveChanges(ResultMode.UploadFiles);
                                    sharePointService.CheckInFile(remoteFile.Url, "", parentList.EnableMinorVersions ? "0" : "1");
                                }
                                else
                                {
                                    MessageBox.Show(@"Failed to upload", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    _repoProvider.SaveChanges(ResultMode.UploadFiles);
                                }
                            }
                            else
                            {
                                MessageBox.Show(@"Fail to check out the file", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                _repoProvider.SaveChanges(ResultMode.UploadFiles);
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show(@"Failed to check out the file", @"Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            _repoProvider.SaveChanges(ResultMode.UploadFiles);
                            return;
                        }
                    }
                    else
                    {
                        // MessageBox.Show("Check out success");
                        _repoProvider.UploadedFiles.Files.Clear();
                        _repoProvider.UploadedFiles.Result.Status = ResultStatus.Pending;
                        _repoProvider.SaveChanges(ResultMode.UploadFiles);

                        sharePointService.UploadFile(remoteFile.Url, fileStream);

                        // MessageBox.Show("Upload " + remoteFile.Url + " success");

                        _repoProvider.UploadedFiles.Result = new Result() { Status = ResultStatus.Success };
                        _repoProvider.SaveChanges(ResultMode.UploadFiles);
                    }
                    fileStream.Close();
                }
                else
                {
                    MessageBox.Show(@"The remote file does not exist", @"Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    _repoProvider.UploadedFiles.Result.Status = ResultStatus.Error;
                    _repoProvider.SaveChanges(ResultMode.UploadFiles);
                }
            }
            catch (Exception ex)
            {
                _repoProvider.SaveChanges(ResultMode.UploadFiles);
                DialogHelper.ShowErrorDialog(ex, Log.LogException(ex));
            }
        }

        private void SelectFilesHandler()
        {
            // Get SharePointFile
            var sharePointFiles =
                fileListView.SelectedItems.Cast<FilesListViewItem>()
                    .Where(s => s.SharePointLibraryItem is SharePointFile).Select(s => s.SharePointLibraryItem as SharePointFile).ToList();

            // Get SharePointFileVersion
            var fileVersions =
                fileVersionListView.SelectedItems.Cast<ListViewItem>().Select(s => s.Tag as SharePointFileVersion).ToList();

            if (!sharePointFiles.Any() && !fileVersions.Any())
            {
                if (MessageBox.Show(@"No files have been selected", @"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    // update the selected files: have no files were selected
                    _repoProvider.SelectedFiles.Files.Clear();
                    _repoProvider.SelectedFiles.Result.Status = ResultStatus.Error;
                    _repoProvider.SelectedFiles.Result.Message = "No files have been selected";
                    _repoProvider.SaveChanges(ResultMode.SelectFiles);
                    //// CLose the form
                    //Close();
                    return;
                }
            }

            // select a file in FileListView
            if (_fileSelected)
            {
                _repoProvider.SelectedFiles = new SelectedFiles()
                {
                    Result = new Result() { Status = ResultStatus.Success },
                    Files = sharePointFiles.Select(s => new SelectedFile()
                    {
                        Name = s.Name,
                        Url = s.RootUrl + s.RelativeUrl,
                        Author = s.Author,
                        RelativeUrl = s.RelativeUrl,
                        LocalId = s.GetSharePointSiteLocalId(),
                        SiteUrl = s.GetSharePointSiteUrl(),
                        IsCurrentVersion = true,
                        UiVersionLabel = s.UIVersionLabel,
                        TreeViewPath = $"{sitesTreeView.SelectedNode.FullPath}\\{s.Name}"
                    }).ToList()

                };
                _repoProvider.SaveChanges(ResultMode.SelectFiles);
                if (sharePointFiles.Any())
                {
                    Log.LogMessage("SELECTED A FILE AT " + sharePointFiles.First().Url, 1);
                }

#if !DEBUG
                Close();
#endif
                return;
            }

            // Select a version in file version list view
            if (_fileVersionSelected)
            {
                // SelectHistoryVersionFile();
                _repoProvider.SelectedFiles = new SelectedFiles()
                {
                    Result = new Result() { Status = ResultStatus.Success },
                    Files = fileVersions.Select(s => new SelectedFile()
                    {
                        Name = s.OriginalFileName,
                        Author = s.CreatedBy,
                        Url = s.SharePointFile.RootUrl + s.SharePointFile.RelativeUrl,
                        RelativeUrl = s.SharePointFile.RelativeUrl,
                        LocalId = s.SharePointFile.GetSharePointSiteLocalId(),
                        SiteUrl = s.SharePointFile.GetSharePointSiteUrl(),
                        IsCurrentVersion = false,
                        UiVersionLabel = s.VersionLabel,
                        FileVersionUrl = s.Url,
                        TreeViewPath = $"{sitesTreeView.SelectedNode.FullPath}\\{s.OriginalFileName}"
                    }).ToList()

                };
                _repoProvider.SaveChanges(ResultMode.SelectFiles);
#if !DEBUG
                Close(); 
#endif
            }
            // return false;
        }

        private async void listItemsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                _fileVersionSelected = false;
                _fileSelected = fileListView.SelectedItems.Count == 1;
                fileVersionListView.SelectedItems.Clear();
                fileVersionListView.Items.Clear();

                if (_fileSelected)
                {
                    var listItem = fileListView.SelectedItems[0] as FilesListViewItem;

                    if (listItem?.SharePointLibraryItem is SharePointFile)
                    {
                        Log.LogMessage("User clicked " + ((SharePointFile)listItem.SharePointLibraryItem).Name + " on " +
                                       ((FilesListView) listItem.ListView).SiteNodeTreeNode.FullPath, 1);
                        var sharePointFile = (SharePointFile)listItem.SharePointLibraryItem;
                        List<SharePointFileVersion> versions;
                        if (!sharePointFile.HadFilledFileVersion)
                        {
                            Log.LogMessage("Load history versions of " + ((SharePointFile)listItem.SharePointLibraryItem).Name + " on " +
                                       ((FilesListView) listItem.ListView).SiteNodeTreeNode.FullPath, 1);
                            var sharePointService = ((FilesListView) listItem.ListView).SiteNodeTreeNode.SharePointService;
                            try
                            {
                                var taskGuid = Guid.NewGuid();
                                ProcessBegin(taskGuid, "Loading history versions ...");
                                versions = (await
                                    sharePointService.GetVersionsAsync(listItem.SharePointLibraryItem.Url,
                                        listItem.SharePointLibraryItem.RelativeUrl)).ToList();
                                ProcessEnd(taskGuid);
                            }
                            catch (Exception ex)
                            {
                                DialogHelper.ShowErrorDialog(ex, Log.LogException(ex));
                                //        var message = ex.InnerException == null ? $"{ex.Message}{Environment.NewLine}{ex.StackTrace}" :
                                //$"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}{Environment.NewLine}{ex.StackTrace}";
                                //        MessageBox.Show(message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                //        Log.LogException(ex);
                                // Close();
                                return;
                            }
                            versions.ForEach(s => s.SharePointFile = sharePointFile);
                            sharePointFile.SharePointFileVersions = versions;
                            sharePointFile.HadFilledFileVersion = true;
                        }
                        else
                        {
                            versions = sharePointFile.SharePointFileVersions.ToList();
                        }

                        // throw new Exception("This is a text exception");

                        // This make sure the app work properly with async/await
                        if (fileListView.SelectedItems.Count == 1 && listItem == fileListView.SelectedItems[0])
                        {
                            versions.OrderByDescending(s => TryParseDouble(s.VersionLabel)).ToList().ForEach(s =>
                            {
                                var listViewItem = new ListViewItem(new[]
                                {
                                    s.VersionLabel,
                                    s.Created.ToString(CultureInfo.InvariantCulture),
                                    s.CreatedBy,
                                    s.Size == null
                                        ? ""
                                        : ByteSize.FromBytes((long) s.Size).ToString("0.00"),
                                    s.CheckInComment,
                                    s.OriginalFileName = (listItem.SharePointLibraryItem as SharePointFile)?.Name
                                })
                                {
                                    Tag = s
                                };
                                fileVersionListView.Items.Add(listViewItem);
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowErrorDialog(ex, Log.LogException(ex));
                // throw;
                //var message = ex.InnerException == null ? $"{ex.Message}{Environment.NewLine}{ex.StackTrace}" :
                //        $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}{Environment.NewLine}{ex.StackTrace}";
                //MessageBox.Show(message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Log.LogException(ex);
            }
        }

        private double TryParseDouble(string text)
        {
            double result;
            double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
            return result;
        }

        /// <summary>
        /// Download many files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void downloadToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!fileListView.SelectedItems.Cast<FilesListViewItem>().Any(s => s.SharePointLibraryItem is SharePointFile))
            {
                MessageBox.Show(@"Can't download a Folder", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // DownloadedFiles data
            _repoProvider.DownloadedFiles.Files.Clear();
            _repoProvider.DownloadedFiles.Result.Status = ResultStatus.Pending;
            _repoProvider.SaveChanges();

            // get DownloadDirectory and create new one if need 

            var folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                var dir = folderBrowser.SelectedPath;
                // Download files that were selected
                var filesListViewItems = fileListView.SelectedItems.Cast<FilesListViewItem>()
                    .Where(s => s.SharePointLibraryItem is SharePointFile).ToList();
                //var tasks = filesListViewItems.Select(s => s.DownloadFileAsync(dir)).ToArray();
                //Task.WaitAll(tasks);
                var taskGuid = Guid.NewGuid();
                ProcessBegin(taskGuid, "Download files ...");

                foreach (var filesListViewItem in filesListViewItems)
                {
                    await filesListViewItem.DownloadFileAsync(dir);
                }
                ProcessEnd(taskGuid);
                MessageBox.Show(@"Download files successfully", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Save DownloadedFiles data
                _repoProvider.DownloadedFiles.Result.Status = ResultStatus.Success;
                _repoProvider.SaveChanges();
            }
        }

        private void fileVersionListView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var selectediItems = fileVersionListView.SelectedItems.Cast<ListViewItem>();
                //ListViewItem item = fileVersionListView.GetItemAt(e.X, e.Y);
                if (selectediItems.Any())
                {
                    historyVersionContextMenuStrip.Show(fileVersionListView, e.Location);
                }
            }
        }

        /// <summary>
        /// Download history version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void downloadToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_repoProvider.DownloadDirectory))
            {
                var dialog = (new FolderBrowserDialog());
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _repoProvider.DownloadDirectory = dialog.SelectedPath;
                    _repoProvider.SaveChanges();
                }
                else
                {
                    return;
                }
            }
            if (!Directory.Exists(_repoProvider.DownloadDirectory))
            {
                Directory.CreateDirectory(_repoProvider.DownloadDirectory);
            }
            // get the files from control
            var selectediItems = fileVersionListView.SelectedItems.Cast<ListViewItem>().ToList();
            if (selectediItems.Any())
            {
                var selectediItem = selectediItems.FirstOrDefault();
                if (selectediItem == null)
                {
                    return;
                }

                // reset DownloadedFile list
                _repoProvider.DownloadedFiles.Files.Clear();
                _repoProvider.DownloadedFiles.Result = new Result() { Status = ResultStatus.Pending };
                _repoProvider.SaveChanges();

                Text += @" - Downloading ...";
                var taskGuid = Guid.NewGuid();
                try
                {
                    ProcessBegin(taskGuid, "Downloading file ...");
                    var fileVersion = (SharePointFileVersion) selectediItem.Tag;
                    var target = Path.Combine(_repoProvider.DownloadDirectory, fileVersion.OriginalFileName);

                    using (var fs = new FileStream(target, FileMode.OpenOrCreate))
                    using (var fileInfo =
                        fileVersionListView.SharePointService.DownloadHistoryVersion(
                            ((SharePointFileVersion)selectediItem.Tag).Url))
                    {
                        await Utils.CopyToAsync(fileInfo, fs);
                    }
                    _repoProvider.DownloadedFiles.Files.Add(new DownloadedFile()
                    {
                        Downloaded = true,
                        Url = fileVersion.Url,
                        Name = fileVersion.OriginalFileName,
                        LocalPath = target
                    });

                    _repoProvider.DownloadedFiles.Result = new Result() { Status = ResultStatus.Success };
                    _repoProvider.SaveChanges(ResultMode.DownloadFiles);

                    MessageBox.Show(@"Download " + fileVersion.OriginalFileName + @" successfully", @"Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    _repoProvider.DownloadedFiles.Result.Status = ResultStatus.Error;
                    _repoProvider.SaveChanges(ResultMode.DownloadFiles);

                    DialogHelper.ShowErrorDialog(ex, Log.LogException(ex));
                    //var message = ex.InnerException == null ? $"{ex.Message}{Environment.NewLine}{ex.StackTrace}" :
                    //    $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}{Environment.NewLine}{ex.StackTrace}";
                    //MessageBox.Show(message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //Log.LogException(ex);
                }
                finally
                {
                    ProcessEnd(taskGuid);
                    Text = Text.Substring(0, Text.Length - " - Downloading ...".Length);
                }
            }
        }


        // Right click context menu in fileListView, refresh the files in the current Library or Folder
        private async void refreshToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            // Cursor = Cursors.WaitCursor;
            // get the selected site node
            var selectedSiteNode = fileListView.SiteNodeTreeNode;
            // refresh site node tree node
            if (selectedSiteNode != null)
            {
                if (selectedSiteNode.IsLoading)
                {
                    return;
                }
                fileListView.SelectedItems.Clear();
                fileListView.Items.Clear();
                var taskGuid = Guid.NewGuid();
                ProcessBegin(taskGuid, $"Refreshing {selectedSiteNode.Text} node ...");
                selectedSiteNode.IsLoading = true;
                await selectedSiteNode.RefreshTreeNodeAsync();

                ProcessEnd(taskGuid);
                // refresh fileListView

                if (sitesTreeView.SelectedNode == selectedSiteNode)
                {
                    RefreshFilesListView(selectedSiteNode);
                }
                selectedSiteNode.IsLoading = false;
            }
            // Cursor = Cursors.Default;
        }

        /// <summary>
        /// Add the selected files to new SeletedFiles list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedFiles =
                fileListView.SelectedItems.Cast<FilesListViewItem>()
                    .Select(s => s.SharePointLibraryItem)
                    .Where(s => s is SharePointFile)
                    .ToList();

            if (selectedFiles.Any())
            {
                _repoProvider.SelectedFiles = new SelectedFiles()
                {
                    Files = selectedFiles.Select(s => new SelectedFile
                    {
                        LocalId = s.GetSharePointSiteLocalId(),
                        Url = s.RootUrl + s.RelativeUrl,
                        RelativeUrl = s.RelativeUrl,
                        Name = s.Name,
                        SiteUrl = s.GetSharePointSiteUrl()
                    }).ToList(),
                    Result = new Result()
                    {
                        Status = ResultStatus.Success
                    }
                };
                _repoProvider.SaveChanges();
                MessageBox.Show(@"The selected files were selected successfully");
            }
            else
            {
                _repoProvider.SelectedFiles.Files.Clear();
                _repoProvider.SelectedFiles.Result.Status = ResultStatus.Success;
                _repoProvider.SelectedFiles.Result.Message = "No files have been selected";
                _repoProvider.SaveChanges();
            }
        }

        /// <summary>
        /// Add seleted files to the current SeletedFiles list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // get selected files from control
            var selectedFiles =
                fileListView.SelectedItems.Cast<FilesListViewItem>()
                    .Select(s => s.SharePointLibraryItem)
                    .Where(s => s is SharePointFile)
                    .ToList();

            // files in the current list
            var filesInList = _repoProvider.SelectedFiles.Files;

            if (selectedFiles.Any())
            {
                // fillter to get the files need to add
                selectedFiles =
                    selectedFiles.Where(
                        s =>
                            filesInList.All(
                                t =>
                                    t.LocalId != s.GetSharePointSiteLocalId() || t.Name != s.Name ||
                                    t.SiteUrl != s.GetSharePointSiteUrl() || t.Url != s.RootUrl + s.RelativeUrl)).ToList();


                _repoProvider.SelectedFiles.Files.AddRange(selectedFiles.Select(s => new SelectedFile()
                {
                    LocalId = s.GetSharePointSiteLocalId(),
                    Url = s.RootUrl + s.RelativeUrl,
                    RelativeUrl = s.RelativeUrl,
                    Name = s.Name,
                    SiteUrl = s.GetSharePointSiteUrl()
                }));
                _repoProvider.SelectedFiles.Result = new Result() { Status = ResultStatus.Success };
                _repoProvider.SaveChanges();
                MessageBox.Show(@"The selected files were added successfully");
            }
            else
            {
                _repoProvider.SelectedFiles.Files.Clear();
                _repoProvider.SelectedFiles.Result.Status = ResultStatus.Success;
                _repoProvider.SelectedFiles.Result.Message = "No files have been selected";
                _repoProvider.SaveChanges();
            }
        }


        /// <summary>
        /// run if only one file was selected. Add the selected file in to new SelectedFile list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var selectedFiles =
                fileListView.SelectedItems.Cast<FilesListViewItem>()
                    .Select(s => s.SharePointLibraryItem)
                    .Where(s => s is SharePointFile).Cast<SharePointFile>().ToList();
            if (selectedFiles.Count == 1)
            {
                _repoProvider.SelectedFiles = new SelectedFiles()
                {
                    Result = new Result() { Status = ResultStatus.Success },
                    Files = new List<SelectedFile>() { selectedFiles.Select(s => new SelectedFile
                    {
                        Url = s.Url,
                        RelativeUrl = s.RelativeUrl,
                        Name = s.Name,
                        Author = s.Author,
                        SiteUrl = s.GetSharePointSiteUrl(),
                        LocalId = s.GetSharePointSiteLocalId(),
                        UiVersionLabel = s.UIVersionLabel,
                        IsCurrentVersion = true,
                        TreeViewPath = $"{sitesTreeView.SelectedNode.FullPath}\\{s.Name}"
                    }).First() }
                };
                _repoProvider.SaveChanges(ResultMode.SelectFiles);
                Log.LogMessage("SELECTED A FILE AT " + selectedFiles.First().Url, 1);
                MessageBox.Show(@"The file was selected successfully", @"Successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Save last selection path
                try
                {
                    var lastSelection = sitesTreeView.SelectedNode.FullPath;
                    Settings.Default.LastSelectionPath = lastSelection;
                    Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    DialogHelper.ShowErrorDialog(ex, Log.LogException(ex));
                    //Log.LogException(ex);
                    // throw;
                }
#if !DEBUG
                Close(); 
#endif
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedFiles =
                fileListView.SelectedItems.Cast<FilesListViewItem>()
                    .Select(s => s.SharePointLibraryItem)
                    .Where(s => s is SharePointFile).ToList();
            if (selectedFiles.Count != 1) return;

            var selectedFile = new SelectedFile
            {
                LocalId = selectedFiles.First().GetSharePointSiteLocalId(),
                Url = selectedFiles.First().RootUrl + selectedFiles.First().RelativeUrl,
                RelativeUrl = selectedFiles.First().RelativeUrl,
                Name = selectedFiles.First().Name,
                SiteUrl = selectedFiles.First().GetSharePointSiteUrl()
            };

            if (!_repoProvider.SelectedFiles.Files.All(s =>
                s.LocalId != selectedFile.LocalId || s.Name != selectedFile.Name ||
                s.SiteUrl != selectedFile.SiteUrl || s.Url != selectedFile.Url)) return;
            _repoProvider.SelectedFiles.Files.Add(selectedFile);
            _repoProvider.SelectedFiles.Result = new Result()
            {
                Status = ResultStatus.Success
            };
            _repoProvider.SaveChanges();
            MessageBox.Show(@"The file was added successfully");
        }

        private async void checkOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileListView.SelectedItems.Count > 1)
            {
                MessageBox.Show(@"Error", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            var listItem = fileListView.SelectedItems[0] as FilesListViewItem;
            if (listItem == null)
            {
                return;
            }
            var taskGuid = Guid.NewGuid();
            ProcessBegin(taskGuid, "Checking out " + listItem.Text + " ...");
            var result = await listItem.CheckOutAsync();
            if (result)
            {
                // Todo can improve this code for performance
                //RefreshFilesListView(fileListView.SiteNodeTreeNode);
                await fileListView.SiteNodeTreeNode.RefreshTreeNodeAsync();
                // refresh fileListView
                RefreshFilesListView(fileListView.SiteNodeTreeNode);
            }
            ProcessEnd(taskGuid);
            if (result)
            {
                MessageBox.Show(@"Check out successfully", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(@"Check out failed", @"Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private async void checkInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileListView.SelectedItems.Count > 1)
            {
                MessageBox.Show(@"Error", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            var listItem = fileListView.SelectedItems[0] as FilesListViewItem;
            if (listItem == null)
            {
                return;
            }

            var taskGuid = Guid.NewGuid();
            ProcessBegin(taskGuid, "Checking in " + listItem.Text + " ...");

            var result = await listItem.CheckInAsync();
            if (result)
            {
                // Todo can improve this code for performance
                //RefreshFilesListView(fileListView.SiteNodeTreeNode);
                await fileListView.SiteNodeTreeNode.RefreshTreeNodeAsync();
                // refresh fileListView
                RefreshFilesListView(fileListView.SiteNodeTreeNode);
            }

            ProcessEnd(taskGuid);
            if (result)
            {
                MessageBox.Show(@"Check in successfully", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(@"Check in fail", @"Fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //MessageBox.Show(result ? @"Success" : @"Fail");
        }

        private void fileVersionListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SelectHistoryVersionFile();
            try
            {
                // Save last selection path
                var lastSelection = sitesTreeView.SelectedNode.FullPath;
                Settings.Default.LastSelectionPath = lastSelection;
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowErrorDialog(ex, Log.LogException(ex));
                //Log.LogException(ex);
                // throw;
            }
        }

        private void selectToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SelectHistoryVersionFile();
            try
            {
                // Save last selection path
                var lastSelection = sitesTreeView.SelectedNode.FullPath;
                Settings.Default.LastSelectionPath = lastSelection;
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowErrorDialog(ex, Log.LogException(ex));
                //Log.LogException(ex);
                // throw;
            }
        }

        private void fileVersionListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            // A file was be selected
            _fileSelected = false;
            _fileVersionSelected = fileVersionListView.SelectedItems.Count == 1;
        }

        private void fileListView_Enter(object sender, EventArgs e)
        {
            _fileVersionSelected = false;
            _fileSelected = fileListView.SelectedItems.Count >= 1;
        }

        private void fileVersionListView_Enter(object sender, EventArgs e)
        {
            _fileSelected = false;
            _fileVersionSelected = fileVersionListView.SelectedItems.Count >= 1;
        }
        private void FileListViewReOrderByColumn()
        {
            fileListView.ListViewItemSorter =
                new FileListViewColumnComparer(GetFileListViewColumn(_lastSortFileListViewColumn),
                    _lastSortFileListViewOrder);
            //if (this.fileListView.ShowGroups)
            //    this.BuildGroups(this.lastSortColumn);
            //else
            //    this.listView1.ListViewItemSorter = new ColumnComparer(this.lastSortColumn, this.lastSortOrder);
        }

        private FileListViewColumns GetFileListViewColumn(int col)
        {
            switch (col)
            {
                case 0:
                    return FileListViewColumns.Name;
                case 1:
                    return FileListViewColumns.Created;
                case 2:
                    return FileListViewColumns.Author;
                case 3:
                    return FileListViewColumns.Size;
                case 4:
                    return FileListViewColumns.CheckOutTo;
                case 5:
                    return FileListViewColumns.LastModified;
            }
            throw new Exception();
        }

        private HistoryFileListViewColumns GetHistoryFileListViewColumns(int col)
        {
            switch (col)
            {
                case 0:
                    return HistoryFileListViewColumns.Version;
                case 1:
                    return HistoryFileListViewColumns.CreatedDate;
                case 2:
                    return HistoryFileListViewColumns.Author;
                case 3:
                    return HistoryFileListViewColumns.Size;
                case 4:
                    return HistoryFileListViewColumns.Comment;
            }
            throw new Exception();
        }

        private void fileVersionListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == _lastSortHistoryFileColumn)
            {
                _lastSortHistoryFileListViewOrder = _lastSortHistoryFileListViewOrder == SortOrder.Ascending
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
            }
            else
            {
                _lastSortHistoryFileListViewOrder = SortOrder.Ascending;
                _lastSortHistoryFileColumn = e.Column;
            }

            HistoryFileListViewReOrderByColumn();
            fileVersionListView.SetSortIcon(e.Column, _lastSortHistoryFileListViewOrder);
        }

        private void HistoryFileListViewReOrderByColumn()
        {
            fileVersionListView.ListViewItemSorter =
                new HistoryFileListViewColumnComparer(GetHistoryFileListViewColumns(_lastSortHistoryFileColumn),
                    _lastSortHistoryFileListViewOrder);
        }

        private void fileListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == _lastSortFileListViewColumn)
            {
                _lastSortFileListViewOrder = _lastSortFileListViewOrder == SortOrder.Ascending
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
            }
            else
            {
                _lastSortFileListViewOrder = SortOrder.Ascending;
                _lastSortFileListViewColumn = e.Column;
            }

            FileListViewReOrderByColumn();
            fileListView.SetSortIcon(e.Column, _lastSortFileListViewOrder);
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            try
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    Settings.Default.FormWindowStateMaximized = true;
                }
                else
                {
                    Settings.Default.Width = Width;
                    Settings.Default.Height = Height;
                }
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                // not should show a dialog here
                Log.LogException(ex);
                // throw;
            }
        }
        protected override void WndProc(ref Message m)
        {
            var previousWindowState = WindowState;

            base.WndProc(ref m);

            var currentWindowState = WindowState;

            if (previousWindowState != currentWindowState)
            {
                try
                {
                    Settings.Default.FormWindowStateMaximized = currentWindowState == FormWindowState.Maximized;
                    Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    // not should show a dialog here
                    Log.LogException(ex);
                    //throw;
                }
            }
        }

        private void edtMe_Validating(object sender, CancelEventArgs e)
        {
            edtMe.Text = edtMe.Text.Trim();
            var validationResult = ValidateFileName(edtMe.Text);
            if (!string.IsNullOrEmpty(validationResult))
            {
                MessageBox.Show(validationResult, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }

        }

        public static string ValidateFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars().ToList();
            invalidChars.Add('\\');
            invalidChars.Add('/');
            invalidChars.Add(':');
            invalidChars.Add('*');
            invalidChars.Add('?');
            invalidChars.Add('"');
            invalidChars.Add('>');
            invalidChars.Add('<');
            invalidChars.Add('|');
            invalidChars.Add('#');
            invalidChars.Add('{');
            invalidChars.Add('}');
            invalidChars.Add('%');
            invalidChars.Add('~');
            invalidChars.Add('&');

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return "File name is required.";
            }

            if (invalidChars.Any(fileName.Contains))
            {
                return "File name contain invalid characters. Please try again.";
            }
            if (fileName.Length > 128)
            {
                return "Maximum number of characters is 128. Please try again.";
            }
            if (string.IsNullOrWhiteSpace(Path.GetFileNameWithoutExtension(fileName)))
            {
                return "You must type a file name";
            }

            return string.Empty;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            toolStripStatusLabel.Margin = new Padding()
            {
                Bottom = toolStripStatusLabel.Margin.Bottom,
                Left = toolStripStatusLabel.Margin.Left,
                Right = Width - 538,
                Top = toolStripStatusLabel.Margin.Top
            };
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check 
            if (sitesTreeView.Enabled)
            {
                // Save last selection path
                try
                {
                    var lastSelection = sitesTreeView.SelectedNode.FullPath;
                    Settings.Default.LastSelectionPath = lastSelection;
                    Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    // Not should show a dialog here
                    Log.LogException(ex);
                }
            }
        }

        private void copyUrlToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var selectediItems = fileVersionListView.SelectedItems.Cast<ListViewItem>().ToList();
            if (selectediItems.Any())
            {
                var fileVersion = (SharePointFileVersion) selectediItems.First().Tag;
                Clipboard.SetText(fileVersion.Url);
            }
        }
    }
}

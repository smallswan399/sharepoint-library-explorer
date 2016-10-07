using Main.UserControls;

namespace Main
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("SharePoint Sites");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.sitesTreeView = new System.Windows.Forms.TreeView();
            this.siteImageList = new System.Windows.Forms.ImageList(this.components);
            this.panel4 = new System.Windows.Forms.Panel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.fileListView = new Main.UserControls.FilesListView();
            this.name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dateTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.author = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.size = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.checkoutTo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lastModify = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.libratyListItemImageList = new System.Windows.Forms.ImageList(this.components);
            this.fileVersionListView = new Main.UserControls.FileVersionListView();
            this.version = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.modified = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.user = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._size = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.comment = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtAdressUrl = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbSaveNewFile = new System.Windows.Forms.RadioButton();
            this.rbSaveVersion = new System.Windows.Forms.RadioButton();
            this.edtMe = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.noFilesWasSelectedContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.rootNodeContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addSiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshRootToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.siteManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.siteContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oneFileWasSelectedContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.downloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.propertyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyUrlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listViewItemsSelectedcontextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.downloadToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.siteNodeContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.historyVersionContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.downloadToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.selectToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.copyUrlToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.imList = new System.Windows.Forms.ImageList(this.components);
            this.imListB = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.noFilesWasSelectedContextMenuStrip.SuspendLayout();
            this.rootNodeContextMenuStrip.SuspendLayout();
            this.siteContextMenuStrip.SuspendLayout();
            this.oneFileWasSelectedContextMenuStrip.SuspendLayout();
            this.listViewItemsSelectedcontextMenuStrip.SuspendLayout();
            this.siteNodeContextMenuStrip.SuspendLayout();
            this.historyVersionContextMenuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.sitesTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel4);
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(992, 568);
            this.splitContainer1.SplitterDistance = 290;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // sitesTreeView
            // 
            this.sitesTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sitesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sitesTreeView.HideSelection = false;
            this.sitesTreeView.ImageIndex = 0;
            this.sitesTreeView.ImageList = this.siteImageList;
            this.sitesTreeView.ItemHeight = 18;
            this.sitesTreeView.Location = new System.Drawing.Point(0, 0);
            this.sitesTreeView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.sitesTreeView.Name = "sitesTreeView";
            treeNode1.Name = "RootNode";
            treeNode1.Text = "SharePoint Sites";
            this.sitesTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.sitesTreeView.SelectedImageIndex = 0;
            this.sitesTreeView.ShowNodeToolTips = true;
            this.sitesTreeView.Size = new System.Drawing.Size(290, 568);
            this.sitesTreeView.TabIndex = 0;
            this.sitesTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.sitesTreeView_BeforeExpand);
            this.sitesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.sitesTreeView_AfterSelect);
            this.sitesTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sitesTreeView_MouseDown);
            // 
            // siteImageList
            // 
            this.siteImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("siteImageList.ImageStream")));
            this.siteImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.siteImageList.Images.SetKeyName(0, "Home.png");
            this.siteImageList.Images.SetKeyName(1, "Server.png");
            this.siteImageList.Images.SetKeyName(2, "Window.png");
            this.siteImageList.Images.SetKeyName(3, "Server-Tick.png");
            this.siteImageList.Images.SetKeyName(4, "Server-Warning.png");
            this.siteImageList.Images.SetKeyName(5, "Server-Add.png");
            this.siteImageList.Images.SetKeyName(6, "Server-Edit.png");
            this.siteImageList.Images.SetKeyName(7, "Server-Delete.png");
            this.siteImageList.Images.SetKeyName(8, "itdl.png");
            this.siteImageList.Images.SetKeyName(9, "Folder-Open.png");
            this.siteImageList.Images.SetKeyName(10, "Window-Warning.png");
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.splitContainer2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 28);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(697, 473);
            this.panel4.TabIndex = 6;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.fileListView);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.fileVersionListView);
            this.splitContainer2.Size = new System.Drawing.Size(697, 473);
            this.splitContainer2.SplitterDistance = 213;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 0;
            // 
            // fileListView
            // 
            this.fileListView.AllowColumnReorder = true;
            this.fileListView.AllowDrop = true;
            this.fileListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fileListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name,
            this.dateTime,
            this.author,
            this.size,
            this.checkoutTo,
            this.lastModify});
            this.fileListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileListView.FullRowSelect = true;
            this.fileListView.HideSelection = false;
            this.fileListView.LargeImageList = this.libratyListItemImageList;
            this.fileListView.Location = new System.Drawing.Point(0, 0);
            this.fileListView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fileListView.MultiSelect = false;
            this.fileListView.Name = "fileListView";
            this.fileListView.RepoProvider = null;
            this.fileListView.SharePointFileContainer = null;
            this.fileListView.SharePointService = null;
            this.fileListView.SiteNodeTreeNode = null;
            this.fileListView.Size = new System.Drawing.Size(695, 211);
            this.fileListView.SmallImageList = this.libratyListItemImageList;
            this.fileListView.TabIndex = 0;
            this.fileListView.UseCompatibleStateImageBehavior = false;
            this.fileListView.View = System.Windows.Forms.View.Details;
            this.fileListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.fileListView_ColumnClick);
            this.fileListView.SelectedIndexChanged += new System.EventHandler(this.listItemsListView_SelectedIndexChanged);
            this.fileListView.Enter += new System.EventHandler(this.fileListView_Enter);
            this.fileListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listItemslistView_MouseDoubleClick);
            this.fileListView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listItemslistView_MouseDown);
            // 
            // name
            // 
            this.name.Text = "Name";
            this.name.Width = 200;
            // 
            // dateTime
            // 
            this.dateTime.Text = "Created Date";
            this.dateTime.Width = 150;
            // 
            // author
            // 
            this.author.Text = "Author";
            this.author.Width = 150;
            // 
            // size
            // 
            this.size.Text = "Size";
            this.size.Width = 90;
            // 
            // checkoutTo
            // 
            this.checkoutTo.Text = "Checkout to";
            this.checkoutTo.Width = 100;
            // 
            // lastModify
            // 
            this.lastModify.Text = "Date Modified";
            this.lastModify.Width = 150;
            // 
            // libratyListItemImageList
            // 
            this.libratyListItemImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("libratyListItemImageList.ImageStream")));
            this.libratyListItemImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.libratyListItemImageList.Images.SetKeyName(0, "icgen.gif");
            this.libratyListItemImageList.Images.SetKeyName(1, "folder.png");
            this.libratyListItemImageList.Images.SetKeyName(2, "ichtm.gif");
            this.libratyListItemImageList.Images.SetKeyName(3, "icpdf.png");
            this.libratyListItemImageList.Images.SetKeyName(4, "ictxt.gif");
            this.libratyListItemImageList.Images.SetKeyName(5, "iczip.gif");
            this.libratyListItemImageList.Images.SetKeyName(6, "icdocx.png");
            this.libratyListItemImageList.Images.SetKeyName(7, "icjpg.gif");
            this.libratyListItemImageList.Images.SetKeyName(8, "icxml.gif");
            this.libratyListItemImageList.Images.SetKeyName(9, "icxlsx.png");
            this.libratyListItemImageList.Images.SetKeyName(10, "icini.gif");
            this.libratyListItemImageList.Images.SetKeyName(11, "icppt.png");
            this.libratyListItemImageList.Images.SetKeyName(12, "icpptx.png");
            this.libratyListItemImageList.Images.SetKeyName(13, "icrtf.gif");
            this.libratyListItemImageList.Images.SetKeyName(14, "icxls.png");
            this.libratyListItemImageList.Images.SetKeyName(15, "icpptm.png");
            this.libratyListItemImageList.Images.SetKeyName(16, "icxltm.png");
            this.libratyListItemImageList.Images.SetKeyName(17, "icxlsm.png");
            this.libratyListItemImageList.Images.SetKeyName(18, "icpps.png");
            this.libratyListItemImageList.Images.SetKeyName(19, "icppsm.png");
            this.libratyListItemImageList.Images.SetKeyName(20, "icppsx.png");
            this.libratyListItemImageList.Images.SetKeyName(21, "icdocm.png");
            this.libratyListItemImageList.Images.SetKeyName(22, "icxlsb.png");
            // 
            // fileVersionListView
            // 
            this.fileVersionListView.AllowColumnReorder = true;
            this.fileVersionListView.AllowDrop = true;
            this.fileVersionListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fileVersionListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.version,
            this.modified,
            this.user,
            this._size,
            this.comment});
            this.fileVersionListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileVersionListView.FullRowSelect = true;
            this.fileVersionListView.HideSelection = false;
            this.fileVersionListView.Location = new System.Drawing.Point(0, 0);
            this.fileVersionListView.MultiSelect = false;
            this.fileVersionListView.Name = "fileVersionListView";
            this.fileVersionListView.SharePointService = null;
            this.fileVersionListView.Size = new System.Drawing.Size(695, 253);
            this.fileVersionListView.TabIndex = 0;
            this.fileVersionListView.UseCompatibleStateImageBehavior = false;
            this.fileVersionListView.View = System.Windows.Forms.View.Details;
            this.fileVersionListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.fileVersionListView_ColumnClick);
            this.fileVersionListView.SelectedIndexChanged += new System.EventHandler(this.fileVersionListView_SelectedIndexChanged);
            this.fileVersionListView.Enter += new System.EventHandler(this.fileVersionListView_Enter);
            this.fileVersionListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.fileVersionListView_MouseDoubleClick);
            this.fileVersionListView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.fileVersionListView_MouseDown);
            // 
            // version
            // 
            this.version.Text = "Version";
            this.version.Width = 100;
            // 
            // modified
            // 
            this.modified.Text = "Created Date";
            this.modified.Width = 200;
            // 
            // user
            // 
            this.user.Text = "Author";
            this.user.Width = 150;
            // 
            // _size
            // 
            this._size.Text = "Size";
            this._size.Width = 100;
            // 
            // comment
            // 
            this.comment.Text = "Comment";
            this.comment.Width = 191;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txtAdressUrl);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(697, 28);
            this.panel2.TabIndex = 5;
            // 
            // txtAdressUrl
            // 
            this.txtAdressUrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAdressUrl.Location = new System.Drawing.Point(0, 0);
            this.txtAdressUrl.Name = "txtAdressUrl";
            this.txtAdressUrl.ReadOnly = true;
            this.txtAdressUrl.Size = new System.Drawing.Size(697, 22);
            this.txtAdressUrl.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbSaveNewFile);
            this.panel1.Controls.Add(this.rbSaveVersion);
            this.panel1.Controls.Add(this.edtMe);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 501);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(697, 67);
            this.panel1.TabIndex = 2;
            // 
            // rbSaveNewFile
            // 
            this.rbSaveNewFile.AutoSize = true;
            this.rbSaveNewFile.Location = new System.Drawing.Point(17, 33);
            this.rbSaveNewFile.Name = "rbSaveNewFile";
            this.rbSaveNewFile.Size = new System.Drawing.Size(130, 20);
            this.rbSaveNewFile.TabIndex = 6;
            this.rbSaveNewFile.Text = "Save as New File:";
            this.rbSaveNewFile.UseVisualStyleBackColor = true;
            // 
            // rbSaveVersion
            // 
            this.rbSaveVersion.AutoSize = true;
            this.rbSaveVersion.Checked = true;
            this.rbSaveVersion.Location = new System.Drawing.Point(17, 7);
            this.rbSaveVersion.Name = "rbSaveVersion";
            this.rbSaveVersion.Size = new System.Drawing.Size(148, 20);
            this.rbSaveVersion.TabIndex = 5;
            this.rbSaveVersion.TabStop = true;
            this.rbSaveVersion.Text = "Save as New Version";
            this.rbSaveVersion.UseVisualStyleBackColor = true;
            // 
            // edtMe
            // 
            this.edtMe.Location = new System.Drawing.Point(153, 33);
            this.edtMe.Name = "edtMe";
            this.edtMe.Size = new System.Drawing.Size(361, 22);
            this.edtMe.TabIndex = 4;
            this.edtMe.Validating += new System.ComponentModel.CancelEventHandler(this.edtMe_Validating);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(538, 23);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(150, 32);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "&OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // noFilesWasSelectedContextMenuStrip
            // 
            this.noFilesWasSelectedContextMenuStrip.Font = new System.Drawing.Font("Arial", 9.75F);
            this.noFilesWasSelectedContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFolderToolStripMenuItem,
            this.newToolStripMenuItem,
            this.refreshToolStripMenuItem3});
            this.noFilesWasSelectedContextMenuStrip.Name = "listItemsContextMenuStrip";
            this.noFilesWasSelectedContextMenuStrip.Size = new System.Drawing.Size(142, 70);
            // 
            // newFolderToolStripMenuItem
            // 
            this.newFolderToolStripMenuItem.Enabled = false;
            this.newFolderToolStripMenuItem.Name = "newFolderToolStripMenuItem";
            this.newFolderToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.newFolderToolStripMenuItem.Text = "New Folder";
            this.newFolderToolStripMenuItem.Click += new System.EventHandler(this.newFolderToolStripMenuItem_Click);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.AutoToolTip = true;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.newToolStripMenuItem.Text = "Upload";
            this.newToolStripMenuItem.ToolTipText = "Upload a file";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem3
            // 
            this.refreshToolStripMenuItem3.Name = "refreshToolStripMenuItem3";
            this.refreshToolStripMenuItem3.Size = new System.Drawing.Size(141, 22);
            this.refreshToolStripMenuItem3.Text = "Refresh";
            this.refreshToolStripMenuItem3.Click += new System.EventHandler(this.refreshToolStripMenuItem3_Click);
            // 
            // rootNodeContextMenuStrip
            // 
            this.rootNodeContextMenuStrip.Font = new System.Drawing.Font("Arial", 9.75F);
            this.rootNodeContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSiteToolStripMenuItem,
            this.refreshRootToolStripMenuItem,
            this.siteManagerToolStripMenuItem});
            this.rootNodeContextMenuStrip.Name = "rootNodeContextMenuStrip";
            this.rootNodeContextMenuStrip.Size = new System.Drawing.Size(154, 70);
            // 
            // addSiteToolStripMenuItem
            // 
            this.addSiteToolStripMenuItem.Name = "addSiteToolStripMenuItem";
            this.addSiteToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.addSiteToolStripMenuItem.Text = "Add site";
            this.addSiteToolStripMenuItem.Click += new System.EventHandler(this.addSiteToolStripMenuItem_Click);
            // 
            // refreshRootToolStripMenuItem
            // 
            this.refreshRootToolStripMenuItem.Name = "refreshRootToolStripMenuItem";
            this.refreshRootToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.refreshRootToolStripMenuItem.Text = "Refresh";
            this.refreshRootToolStripMenuItem.Click += new System.EventHandler(this.refreshRootToolStripMenuItem_Click);
            // 
            // siteManagerToolStripMenuItem
            // 
            this.siteManagerToolStripMenuItem.Name = "siteManagerToolStripMenuItem";
            this.siteManagerToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.siteManagerToolStripMenuItem.Text = "Site manager";
            this.siteManagerToolStripMenuItem.Click += new System.EventHandler(this.siteManagerToolStripMenuItem_Click);
            // 
            // siteContextMenuStrip
            // 
            this.siteContextMenuStrip.Font = new System.Drawing.Font("Arial", 9.75F);
            this.siteContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.editToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.siteContextMenuStrip.Name = "siteContextMenuStrip";
            this.siteContextMenuStrip.Size = new System.Drawing.Size(121, 70);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // oneFileWasSelectedContextMenuStrip
            // 
            this.oneFileWasSelectedContextMenuStrip.Font = new System.Drawing.Font("Arial", 9.75F);
            this.oneFileWasSelectedContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadToolStripMenuItem,
            this.selectToolStripMenuItem1,
            this.addToolStripMenuItem,
            this.deleteToolStripMenuItem1,
            this.propertyToolStripMenuItem,
            this.copyUrlToolStripMenuItem,
            this.checkOutToolStripMenuItem,
            this.checkInToolStripMenuItem});
            this.oneFileWasSelectedContextMenuStrip.Name = "listViewItemsContextMenuStrip";
            this.oneFileWasSelectedContextMenuStrip.Size = new System.Drawing.Size(139, 180);
            // 
            // downloadToolStripMenuItem
            // 
            this.downloadToolStripMenuItem.AutoToolTip = true;
            this.downloadToolStripMenuItem.Name = "downloadToolStripMenuItem";
            this.downloadToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.downloadToolStripMenuItem.Text = "Download";
            this.downloadToolStripMenuItem.ToolTipText = "Download file";
            this.downloadToolStripMenuItem.Click += new System.EventHandler(this.downloadToolStripMenuItem_Click);
            // 
            // selectToolStripMenuItem1
            // 
            this.selectToolStripMenuItem1.Name = "selectToolStripMenuItem1";
            this.selectToolStripMenuItem1.Size = new System.Drawing.Size(138, 22);
            this.selectToolStripMenuItem1.Text = "Select";
            this.selectToolStripMenuItem1.ToolTipText = "Add file to new Selected list";
            this.selectToolStripMenuItem1.Click += new System.EventHandler(this.selectToolStripMenuItem1_Click);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.AutoToolTip = true;
            this.addToolStripMenuItem.Enabled = false;
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.ToolTipText = "Add file to current Selected list";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Enabled = false;
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(138, 22);
            this.deleteToolStripMenuItem1.Text = "Delete";
            // 
            // propertyToolStripMenuItem
            // 
            this.propertyToolStripMenuItem.Enabled = false;
            this.propertyToolStripMenuItem.Name = "propertyToolStripMenuItem";
            this.propertyToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.propertyToolStripMenuItem.Text = "Property";
            // 
            // copyUrlToolStripMenuItem
            // 
            this.copyUrlToolStripMenuItem.Name = "copyUrlToolStripMenuItem";
            this.copyUrlToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.copyUrlToolStripMenuItem.Text = "Copy Url";
            this.copyUrlToolStripMenuItem.Click += new System.EventHandler(this.copyUrlToolStripMenuItem_Click);
            // 
            // checkOutToolStripMenuItem
            // 
            this.checkOutToolStripMenuItem.Name = "checkOutToolStripMenuItem";
            this.checkOutToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.checkOutToolStripMenuItem.Text = "Check Out";
            this.checkOutToolStripMenuItem.Click += new System.EventHandler(this.checkOutToolStripMenuItem_Click);
            // 
            // checkInToolStripMenuItem
            // 
            this.checkInToolStripMenuItem.Name = "checkInToolStripMenuItem";
            this.checkInToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.checkInToolStripMenuItem.Text = "Check In";
            this.checkInToolStripMenuItem.Click += new System.EventHandler(this.checkInToolStripMenuItem_Click);
            // 
            // listViewItemsSelectedcontextMenuStrip
            // 
            this.listViewItemsSelectedcontextMenuStrip.Font = new System.Drawing.Font("Arial", 9.75F);
            this.listViewItemsSelectedcontextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadToolStripMenuItem1,
            this.selectToolStripMenuItem,
            this.addToolStripMenuItem1});
            this.listViewItemsSelectedcontextMenuStrip.Name = "listViewItemsSelectedcontextMenuStrip";
            this.listViewItemsSelectedcontextMenuStrip.Size = new System.Drawing.Size(133, 70);
            // 
            // downloadToolStripMenuItem1
            // 
            this.downloadToolStripMenuItem1.AutoToolTip = true;
            this.downloadToolStripMenuItem1.Name = "downloadToolStripMenuItem1";
            this.downloadToolStripMenuItem1.Size = new System.Drawing.Size(132, 22);
            this.downloadToolStripMenuItem1.Text = "Download";
            this.downloadToolStripMenuItem1.Click += new System.EventHandler(this.downloadToolStripMenuItem1_Click);
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.AutoToolTip = true;
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            this.selectToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.selectToolStripMenuItem.Text = "Select";
            this.selectToolStripMenuItem.ToolTipText = "Add files to new Selected list";
            this.selectToolStripMenuItem.Click += new System.EventHandler(this.selectToolStripMenuItem_Click);
            // 
            // addToolStripMenuItem1
            // 
            this.addToolStripMenuItem1.AutoToolTip = true;
            this.addToolStripMenuItem1.Name = "addToolStripMenuItem1";
            this.addToolStripMenuItem1.Size = new System.Drawing.Size(132, 22);
            this.addToolStripMenuItem1.Text = "Add";
            this.addToolStripMenuItem1.ToolTipText = "Add files to current Selected list";
            this.addToolStripMenuItem1.Click += new System.EventHandler(this.addToolStripMenuItem1_Click);
            // 
            // siteNodeContextMenuStrip
            // 
            this.siteNodeContextMenuStrip.Font = new System.Drawing.Font("Arial", 9.75F);
            this.siteNodeContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem2});
            this.siteNodeContextMenuStrip.Name = "siteItemContextMenuStrip";
            this.siteNodeContextMenuStrip.Size = new System.Drawing.Size(121, 26);
            // 
            // refreshToolStripMenuItem2
            // 
            this.refreshToolStripMenuItem2.Name = "refreshToolStripMenuItem2";
            this.refreshToolStripMenuItem2.Size = new System.Drawing.Size(120, 22);
            this.refreshToolStripMenuItem2.Text = "Refresh";
            this.refreshToolStripMenuItem2.Click += new System.EventHandler(this.refreshToolStripMenuItem2_Click);
            // 
            // historyVersionContextMenuStrip
            // 
            this.historyVersionContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadToolStripMenuItem2,
            this.selectToolStripMenuItem2,
            this.copyUrlToolStripMenuItem1});
            this.historyVersionContextMenuStrip.Name = "contextMenuStripHistoryVersion";
            this.historyVersionContextMenuStrip.Size = new System.Drawing.Size(129, 70);
            // 
            // downloadToolStripMenuItem2
            // 
            this.downloadToolStripMenuItem2.Name = "downloadToolStripMenuItem2";
            this.downloadToolStripMenuItem2.Size = new System.Drawing.Size(128, 22);
            this.downloadToolStripMenuItem2.Text = "Download";
            this.downloadToolStripMenuItem2.Click += new System.EventHandler(this.downloadToolStripMenuItem2_Click);
            // 
            // selectToolStripMenuItem2
            // 
            this.selectToolStripMenuItem2.Name = "selectToolStripMenuItem2";
            this.selectToolStripMenuItem2.Size = new System.Drawing.Size(128, 22);
            this.selectToolStripMenuItem2.Text = "Select";
            this.selectToolStripMenuItem2.Click += new System.EventHandler(this.selectToolStripMenuItem2_Click);
            // 
            // copyUrlToolStripMenuItem1
            // 
            this.copyUrlToolStripMenuItem1.Name = "copyUrlToolStripMenuItem1";
            this.copyUrlToolStripMenuItem1.Size = new System.Drawing.Size(128, 22);
            this.copyUrlToolStripMenuItem1.Text = "Copy Url";
            this.copyUrlToolStripMenuItem1.Click += new System.EventHandler(this.copyUrlToolStripMenuItem1_Click);
            // 
            // imList
            // 
            this.imList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imList.ImageStream")));
            this.imList.TransparentColor = System.Drawing.Color.Fuchsia;
            this.imList.Images.SetKeyName(0, "Server-Monitor.bmp");
            this.imList.Images.SetKeyName(1, "Server.bmp");
            this.imList.Images.SetKeyName(2, "shareitem.png");
            this.imList.Images.SetKeyName(3, "sharefolder.png");
            this.imList.Images.SetKeyName(4, "Server-Error.bmp");
            this.imList.Images.SetKeyName(5, "sharegroup.png");
            // 
            // imListB
            // 
            this.imListB.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imListB.ImageStream")));
            this.imListB.TransparentColor = System.Drawing.Color.Fuchsia;
            this.imListB.Images.SetKeyName(0, "Doc-Acrobat.bmp");
            this.imListB.Images.SetKeyName(1, "Doc-Excel.bmp");
            this.imListB.Images.SetKeyName(2, "Doc-HTML.bmp");
            this.imListB.Images.SetKeyName(3, "Doc-Image.bmp");
            this.imListB.Images.SetKeyName(4, "Doc-Outlook.bmp");
            this.imListB.Images.SetKeyName(5, "Doc-RTF.bmp");
            this.imListB.Images.SetKeyName(6, "Doc-Word.bmp");
            this.imListB.Images.SetKeyName(7, "Doc-PPT.png");
            this.imListB.Images.SetKeyName(8, "icppt.png");
            this.imListB.Images.SetKeyName(9, "icpptx.png");
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.toolStripProgressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 568);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(992, 22);
            this.statusStrip.TabIndex = 7;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.AutoSize = false;
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(400, 17);
            this.toolStripStatusLabel.Text = "Ready";
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // MainForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(992, 590);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip);
            this.Font = new System.Drawing.Font("Arial", 9.75F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SharePoint DMS";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.noFilesWasSelectedContextMenuStrip.ResumeLayout(false);
            this.rootNodeContextMenuStrip.ResumeLayout(false);
            this.siteContextMenuStrip.ResumeLayout(false);
            this.oneFileWasSelectedContextMenuStrip.ResumeLayout(false);
            this.listViewItemsSelectedcontextMenuStrip.ResumeLayout(false);
            this.siteNodeContextMenuStrip.ResumeLayout(false);
            this.historyVersionContextMenuStrip.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView sitesTreeView;
        private System.Windows.Forms.ContextMenuStrip rootNodeContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addSiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshRootToolStripMenuItem;
        private System.Windows.Forms.ImageList siteImageList;
        private System.Windows.Forms.ImageList libratyListItemImageList;
        private System.Windows.Forms.ContextMenuStrip siteContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private FilesListView fileListView;
        private System.Windows.Forms.ColumnHeader name;
        private System.Windows.Forms.ColumnHeader dateTime;
        private System.Windows.Forms.ColumnHeader author;
        private System.Windows.Forms.ColumnHeader size;
        private System.Windows.Forms.ContextMenuStrip noFilesWasSelectedContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem newFolderToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip oneFileWasSelectedContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem downloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip listViewItemsSelectedcontextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem propertyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyUrlToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip siteNodeContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem siteManagerToolStripMenuItem;
        private System.Windows.Forms.Button btnOk;
        private FileVersionListView fileVersionListView;
        private System.Windows.Forms.ColumnHeader version;
        private System.Windows.Forms.ColumnHeader modified;
        private System.Windows.Forms.ColumnHeader user;
        private System.Windows.Forms.ColumnHeader _size;
        private System.Windows.Forms.ColumnHeader comment;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtAdressUrl;
        private System.Windows.Forms.ContextMenuStrip historyVersionContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem downloadToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem3;
        private System.Windows.Forms.ImageList imList;
        private System.Windows.Forms.ImageList imListB;
        private System.Windows.Forms.ColumnHeader checkoutTo;
        private System.Windows.Forms.ToolStripMenuItem checkOutToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader lastModify;
        private System.Windows.Forms.ToolStripMenuItem checkInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem2;
        private System.Windows.Forms.TextBox edtMe;
        private System.Windows.Forms.RadioButton rbSaveNewFile;
        private System.Windows.Forms.RadioButton rbSaveVersion;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripMenuItem copyUrlToolStripMenuItem1;
    }
}
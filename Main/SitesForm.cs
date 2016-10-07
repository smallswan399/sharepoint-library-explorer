using System;
using System.Linq;
using System.Windows.Forms;
using Core.Libs;
using Entities;
using Main.Core;
using Main.Libs;
using Main.Repos;
using Main.Services;
using Services;

namespace Main
{
    public partial class SitesForm : Form
    {
        //public ISharePointService SharePointService { get; private set; }
        private readonly IRepoProvider _repoProvider;
        public SitesForm(IRepoProvider repoProvider)
        {
            this._repoProvider = repoProvider;
            InitializeComponent();
//#if !DEBUG
//            this.TopMost = true;
//#endif
            Log.LogMessage("Construct SitesForm to display site list");
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void IndexForm_Load(object sender, EventArgs e)
        {
            siteListView.Columns.Add("Enable", "Enable", 60);
            siteListView.Columns.Add("URL", "Server URL", 200);
            siteListView.Columns.Add("Name", "Server Name", 200);
            siteListView.Columns.Add("User", "User", 200);
            siteListView.Columns.Add("Description", "Description", 200);
            ReloadListView();
            //
        }
        
        private void SitesForm_Shown(object sender, EventArgs e)
        {
            if (siteListView.CanFocus)
            {
                siteListView.Focus();
                //
                if (siteListView.Items.Count >= 1)
                {
                    siteListView.Items[0].Selected = true;
                    siteListView.Select();
                }
            }
        }

        private void testConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedItem = siteListView.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
            if (selectedItem != null)
            {
                var siteId = (Guid)selectedItem.Tag;
                var site = _repoProvider.SiteRepository.GetById(siteId);
                if (site != null)
                {
                    var sharePointService = SharepointServiceProvider.GetSharePointService(new SiteSoapClientMapper()
                    {
                        Url = site.Url,
                        Id = site.Id,
                        RootUrl = site.RootUrl
                    }, site.SharePointServerVersion, site.Credential.ToClearTextCredential());
                    Cursor = Cursors.WaitCursor;
                    var testResult = sharePointService.TestConnection();
                    Cursor = Cursors.Default;
                    if (!testResult.Result)
                    {
                        MessageBox.Show(testResult.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(@"Success", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void siteListView_MouseDown(object sender, MouseEventArgs e)
        {
            ListViewItem item = siteListView.GetItemAt(e.X, e.Y);
            if (item != null && !siteListView.SelectedItems.Contains(item))
            {
                item.Selected = true;

            }
            
            if (e.Button == MouseButtons.Right)
            {
                if (siteListView.SelectedItems.Count == 0)
                {
                    listViewContextMenuStrip.Show(siteListView, e.Location);
                }
                else
                {
                    listViewItemContextMenuStrip.Show(siteListView, e.Location);
                }
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedItem = siteListView.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
            if (selectedItem != null)
            {
                var siteId = (Guid)selectedItem.Tag;
                var site = _repoProvider.SiteRepository.GetById(siteId);
                if (site != null)
                {
                    var siteDetails = site.ToSiteDetails();
                    var siteEdit = new SiteDetailsForm(siteDetails, CrudModel.Edit,
                SharepointServiceProvider.GetSharePointService(new SiteSoapClientMapper()
                {
                    Id = siteDetails.Id,
                    Url = siteDetails.Url,
                    // RootUrl = siteDetails.Url
                }, siteDetails.SharePointServerVersion, siteDetails.Credential));

                    if (siteEdit.ShowDialog() == DialogResult.OK)
                    {
                        siteDetails = siteEdit.Model;
                        //var site = repoProvider.SiteRepository.GetById(siteDetails.Id);
                        site = siteDetails.ToSite(site);
                        _repoProvider.SaveChanges();
                        ReloadListView();
                    }
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedItem = siteListView.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
            if (selectedItem != null)
            {
                var siteId = (Guid)selectedItem.Tag;
                var site = _repoProvider.SiteRepository.GetById(siteId);
                if (site != null)
                {
                    var siteDetails = site.ToSiteDetails();
                    var siteEdit = new SiteDetailsForm(siteDetails, CrudModel.Delete,
                SharepointServiceProvider.GetSharePointService(new SiteSoapClientMapper()
                {
                    Id = siteDetails.Id,
                    Url = siteDetails.Url,
                    // RootUrl = siteDetails.Url
                }, siteDetails.SharePointServerVersion, siteDetails.Credential));

                    if (siteEdit.ShowDialog() == DialogResult.OK)
                    {
                        _repoProvider.SiteRepository.Delete(site);
                        _repoProvider.SaveChanges();
                        ReloadListView();
                    }
                }
            }
        }

        private void ReloadListView()
        {
            siteListView.Items.Clear();
            // load sites from database
            var sites = _repoProvider.SiteRepository.GetAll().OrderBy(s => s.Name);
            var model = sites.ToSiteListItem();
            model.ToList().ForEach(s => siteListView.Items.Add(new ListViewItem(new[] { s.Enable, s.Url, s.Name, s.CredentialUsername, s.Description }, 0)
            {
                Tag = s.Id
            }));


            UpdateButtons();
        }


        private void UpdateButtons() {
            bool bEnabled = (siteListView.Items.Count >= 1);
            btnEdit.Enabled = bEnabled;
            btnCopy.Enabled = bEnabled;
            btnDelete.Enabled = bEnabled;
        }

        private void addSiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var addSiteDialog = new SiteDetailsForm(null, CrudModel.Add, null);
            if (addSiteDialog.ShowDialog() == DialogResult.OK)
            {
                var newSite = addSiteDialog.Model;
                _repoProvider.SiteRepository.Add(new Site()
                {
                    Credential = new Credential()
                    {
                        DomainName = newSite.Credential.DomainName,
                        Password = newSite.Credential.Password,
                        ReTypePassword = newSite.Credential.ReTypePassword,
                        Username = newSite.Credential.Username,
                        PrivateActiveDirectory = newSite.Credential.PrivateActiveDirectory,
                        SharePointAuthenticationOption = newSite.Credential.SharePointAuthenticationOption
                    },
                    Name = newSite.Name,
                    Enable = newSite.Enable,
                    Description = newSite.Description,
                    RequireAuthentication = newSite.RequireAuthentication,
                    Url = newSite.Url,
                    IncludeSubSites = newSite.IncludeSubSites,
                    SharePointServerVersion = newSite.SharePointServerVersion
                });
                _repoProvider.SaveChanges();
                ReloadListView();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var addSiteDialog = new SiteDetailsForm(null, CrudModel.Add, null);
            // if DialogResult.OK => add site to the data.xml. Not include check site connection
            if (addSiteDialog.ShowDialog() == DialogResult.OK)
            {
                var newSite = addSiteDialog.Model;
                _repoProvider.SiteRepository.Add(new Site()
                {
                    Credential = newSite.Credential.ToCredential(),
                    Name = newSite.Name,
                    Enable = newSite.Enable,
                    Description = newSite.Description,
                    RequireAuthentication = newSite.RequireAuthentication,
                    Url = newSite.Url,
                    SharePointServerVersion = newSite.SharePointServerVersion,
                    IncludeSubSites = newSite.IncludeSubSites
                });
                _repoProvider.SaveChanges();
                ReloadListView();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var selectedItem = siteListView.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
            if (selectedItem != null)
            {
                var siteId = (Guid)selectedItem.Tag;
                var site = _repoProvider.SiteRepository.GetById(siteId);
                if (site != null)
                {
                    var siteDetails = site.ToSiteDetails();
                    var siteEdit = new SiteDetailsForm(siteDetails, CrudModel.Edit,
                        SharepointServiceProvider.GetSharePointService(new SiteSoapClientMapper()
                        {
                            Id = siteDetails.Id,
                            Url = siteDetails.Url,
                            // RootUrl = siteDetails.Url
                        }, siteDetails.SharePointServerVersion, siteDetails.Credential));

                    if (siteEdit.ShowDialog() == DialogResult.OK)
                    {
                        siteDetails = siteEdit.Model;
                        //var site = repoProvider.SiteRepository.GetById(siteDetails.Id);
                        site = siteDetails.ToSite(site);
                        _repoProvider.SaveChanges();
                        ReloadListView();
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var selectedItem = siteListView.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
            if (selectedItem != null)
            {
                var siteId = (Guid)selectedItem.Tag;
                var site = _repoProvider.SiteRepository.GetById(siteId);
                if (site != null)
                {
                    var siteDetails = site.ToSiteDetails();
                    var siteEdit = new SiteDetailsForm(siteDetails, CrudModel.Delete,
                SharepointServiceProvider.GetSharePointService(new SiteSoapClientMapper()
                {
                    Id = siteDetails.Id,
                    Url = siteDetails.Url,
                    // RootUrl = siteDetails.Url
                }, siteDetails.SharePointServerVersion, siteDetails.Credential));

                    if (siteEdit.ShowDialog() == DialogResult.OK)
                    {
                        _repoProvider.SiteRepository.Delete(site);
                        _repoProvider.SaveChanges();
                        ReloadListView();
                    }
                }
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            var selectedItem = siteListView.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
            if (selectedItem != null)
            {
                var siteId = (Guid)selectedItem.Tag;
                var site = _repoProvider.SiteRepository.GetById(siteId);
                if (site != null)
                {
                    var siteDetails = site.ToSiteDetails();
                    var siteCopy = new SiteDetailsForm(siteDetails, CrudModel.Copy,
                SharepointServiceProvider.GetSharePointService(new SiteSoapClientMapper()
                {
                    Id = siteDetails.Id,
                    Url = siteDetails.Url,
                    // RootUrl = siteDetails.Url
                }, siteDetails.SharePointServerVersion, siteDetails.Credential));

                    if (siteCopy.ShowDialog() == DialogResult.OK)
                    {

                        siteDetails = siteCopy.Model;
                        //var site = repoProvider.SiteRepository.GetById(siteDetails.Id);
                        site = siteDetails.ToSite();

                        _repoProvider.SiteRepository.Add(site);
                        _repoProvider.SaveChanges();
                        ReloadListView();
                    }
                }
            }
        }

        private void siteListView_DoubleClick(object sender, EventArgs e)
        {
            btnEdit_Click(sender, e);
        }

        private void pnlBottom_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}

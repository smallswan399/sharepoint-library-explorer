using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Core;
using Core.Libs;
using Entities;
using Main.Libs;
using Main.Services;
using Main.Services.Domains;
using Main.ViewModels;
using Microsoft.SharePoint.Client;
using Services;
using Form = System.Windows.Forms.Form;

namespace Main
{
    public partial class SiteDetailsForm : Form
    {
        public SiteDetails Model { get; private set; }
        private readonly CrudModel crudModel;
        public ISharePointService SharePointService { get; private set; }
        public SiteDetailsForm(SiteDetails model, CrudModel crudModel, ISharePointService sharePointService)
        {
            InitializeComponent();

//#if !DEBUG
//            this.TopMost = true;
//#endif

            SharePointService = sharePointService;
            this.crudModel = crudModel;

            // set model value
            Model = model;

            // Fill items to the authentication option combobox
            authenticationOptionComboBox.Items.AddRange(
                SharePointAuthenticationOption.RequireFormsAuthentication.GetEnumListItems()
                    .Select(s => new SharePointAuthenticationOptionComboBoxListItem { Text = s.GetDescription(), Value = s }).ToArray());
            authenticationOptionComboBox.DisplayMember = "Text";
            authenticationOptionComboBox.ValueMember = "Value";

            serverVersionComboBox.Items.AddRange(
                SharePointServerVersion.SharePoint2007.GetEnumListItems().Select(s => new SharePointServerVersionListBoxItem
                {
                    Text = s.GetDescription(),
                    SharePointServerVersion = s
                }).ToArray());

            serverVersionComboBox.DisplayMember = "Text";
            serverVersionComboBox.ValueMember = "SharePointServerVersion";

            var serverVersionComboBoxItems = serverVersionComboBox.Items.Cast<SharePointServerVersionListBoxItem>();

            serverVersionComboBox.SelectedItem =
                serverVersionComboBoxItems
                    .FirstOrDefault(s => s.SharePointServerVersion == SharePointServerVersion.SharePoint2013) ??
                serverVersionComboBoxItems.FirstOrDefault();
        }

        private void AddSite_Load(object sender, EventArgs e)
        {
            if ((crudModel == CrudModel.Delete || crudModel == CrudModel.Edit || crudModel == CrudModel.Copy))
            {
                // fill data from the model to the controls
                if (Model == null)
                {
                    throw new ArgumentException();
                }
                switch (crudModel)
                {
                    case CrudModel.Delete:
                        Text = @"Delete Site";
                        serverNameTextBox.Enabled = false;
                        authenticationGroupBox.Enabled = false;
                        serverUrlTextBox.Enabled = false;
                        serverDescriptionTextBox.Enabled = false;
                        enableServerCheckBox.Enabled = false;
                        loadSubSitesCheckBox.Enabled = false;
                        testButton.Visible = false;
                        serverVersionComboBox.Enabled = false;
                        okButton.Text = @"Delete";
                        break;
                    case CrudModel.Copy:
                        Text = @"Copy Site";
                        okButton.Text = @"OK";
                        break;
                    case CrudModel.Edit:
                        Text = @"Edit Site";
                        okButton.Text = @"OK";
                        break;
                }
                authenticationOptionComboBox.SelectedItem =
                    authenticationOptionComboBox.Items.Cast<SharePointAuthenticationOptionComboBoxListItem>()
                        .First(s => s.Value == Model.Credential.SharePointAuthenticationOption);

                serverVersionComboBox.SelectedItem =
                    serverVersionComboBox.Items.Cast<SharePointServerVersionListBoxItem>()
                        .First(s => s.SharePointServerVersion == Model.SharePointServerVersion);

                serverNameTextBox.Text = Model.Name;
                serverUrlTextBox.Text = Model.Url;
                serverDescriptionTextBox.Text = Model.Description;
                enableServerCheckBox.Checked = Model.Enable;
                loadSubSitesCheckBox.Checked = Model.IncludeSubSites;
                requireAutheticationCheckBox.Checked = Model.RequireAuthentication;
                domainTextBox.Text = Model.Credential.DomainName;
                usernameTextBox.Text = Model.Credential.Username;
                passwordTextBox.Text = Model.Credential.Password;
                rePasswordTextBox.Text = Model.Credential.ReTypePassword;
                privateActiveDirectorycheckBox.Checked = Model.Credential.PrivateActiveDirectory;
            }
            else
            {
                authenticationOptionComboBox.SelectedIndex = 0;
            }



        }

        private void requireAutheticationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            authenticationGroupBox.Enabled = requireAutheticationCheckBox.Checked;
        }

        #region Helper methods

        /// <summary>
        /// Fill data from control to the model
        /// </summary>
        /// <returns></returns>
        private SiteDetails FillModelData()
        {
            if (Model == null)
            {
                return new SiteDetails()
                {
                    Enable = enableServerCheckBox.Checked,
                    IncludeSubSites = loadSubSitesCheckBox.Checked,
                    Name = serverNameTextBox.Text,
                    Description = serverDescriptionTextBox.Text,
                    Credential = new ClearTextCredential()
                    {
                        DomainName = domainTextBox.Text,
                        Password = passwordTextBox.Text,
                        Username = usernameTextBox.Text,
                        ReTypePassword = rePasswordTextBox.Text,
                        PrivateActiveDirectory = privateActiveDirectorycheckBox.Checked,
                        SharePointAuthenticationOption = ((SharePointAuthenticationOptionComboBoxListItem)authenticationOptionComboBox.SelectedItem).Value
                    },
                    Url = Utils.RemoveAllSlashAtFinish(serverUrlTextBox.Text.ToLower().Trim()),
                    RequireAuthentication = requireAutheticationCheckBox.Checked,
                    SharePointServerVersion = (serverVersionComboBox.SelectedItem as SharePointServerVersionListBoxItem).SharePointServerVersion
                };
            }

            Model.Enable = enableServerCheckBox.Checked;
            Model.IncludeSubSites = loadSubSitesCheckBox.Checked;
            Model.Name = serverNameTextBox.Text;
            Model.Description = serverDescriptionTextBox.Text;
            Model.SharePointServerVersion =
                (serverVersionComboBox.SelectedItem as SharePointServerVersionListBoxItem).SharePointServerVersion;
            Model.Credential.PrivateActiveDirectory = privateActiveDirectorycheckBox.Checked;
            Model.Credential.SharePointAuthenticationOption =
                ((SharePointAuthenticationOptionComboBoxListItem)authenticationOptionComboBox.SelectedItem).Value;
            Model.Credential = new ClearTextCredential()
            {
                DomainName = domainTextBox.Text,
                Password = passwordTextBox.Text,
                Username = usernameTextBox.Text,
                ReTypePassword = rePasswordTextBox.Text,
                PrivateActiveDirectory = privateActiveDirectorycheckBox.Checked,
                SharePointAuthenticationOption = ((SharePointAuthenticationOptionComboBoxListItem)authenticationOptionComboBox.SelectedItem).Value
            };
            Model.Url = Utils.RemoveAllSlashAtFinish(serverUrlTextBox.Text.ToLower().Trim());
            Model.RequireAuthentication = requireAutheticationCheckBox.Checked;
            return Model;
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            var text = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            if (string.IsNullOrWhiteSpace(text))
            {
                text = Environment.UserDomainName.ToString(CultureInfo.InvariantCulture);
            }
            domainTextBox.Text = text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            usernameTextBox.Text = Environment.UserName;
        }

        /// <summary>
        /// Test a sharepoint connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void testButton_Click(object sender, EventArgs e)
        {
            Model = FillModelData();
            // Validate the model
            var validationContext = new ValidationContext(Model, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(Model, validationContext, validationResults, true);

            var validatable = Model as IValidatableObject;
            if (validatable != null)
                validationResults.AddRange(validatable.Validate(validationContext));

            try
            {
                var context = new ClientContext(Model.Url);
            }
            catch (Exception ex)
            {
                validationResults.Add(new ValidationResult(string.Format("Invalid Url ({0})", ex.Message)));
            }

            if (!validationResults.Any())
            {
                SharePointService = SharepointServiceProvider.GetSharePointService(new SiteSoapClientMapper()
                {
                    Url = Model.Url,
                    Id = Model.Id,
                    // RootUrl = Model.Url
                }, Model.SharePointServerVersion,
                        new ClearTextCredential()
                        {
                            DomainName = Model.Credential.DomainName,
                            Password = Model.Credential.Password,
                            PrivateActiveDirectory = Model.Credential.PrivateActiveDirectory,
                            SharePointAuthenticationOption = Model.Credential.SharePointAuthenticationOption,
                            ReTypePassword = Model.Credential.ReTypePassword,
                            Username = Model.Credential.Username
                        });

                Cursor = Cursors.WaitCursor;
                var testResult = SharePointService.TestConnection();
                Cursor = Cursors.Default;
                if (!testResult.Result)
                {
                    validationResults.Add(new ValidationResult(testResult.Message));
                }
                else
                {
                    MessageBox.Show(@"Success", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            };

            var stringBuilder = new StringBuilder();
            var i = 0;
            stringBuilder.AppendLine("Invalid input data");
            foreach (var validationResult in validationResults)
            {
                i++;
                stringBuilder.AppendLine(string.Format("{0}) {1}", i, validationResult.ErrorMessage));
            }
            MessageBox.Show(stringBuilder.ToString(), @"Invalid input data", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void authenticationOptionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var authenticationOption =
                ((SharePointAuthenticationOptionComboBoxListItem) authenticationOptionComboBox.SelectedItem).Value;
            privateActiveDirectorycheckBox.Visible = authenticationOption ==
                                                         SharePointAuthenticationOption.ActiveDirectory;
            if (privateActiveDirectorycheckBox.Visible)
            {
                domainTextBox.Text = string.Empty;
                usernameTextBox.Text = string.Empty;
                passwordTextBox.Text = string.Empty;
                rePasswordTextBox.Text = string.Empty;
            }
            domainTextBox.Enabled = !privateActiveDirectorycheckBox.Visible;
            usernameTextBox.Enabled = !privateActiveDirectorycheckBox.Visible;
            passwordTextBox.Enabled = !privateActiveDirectorycheckBox.Visible;
            rePasswordTextBox.Enabled = !privateActiveDirectorycheckBox.Visible;
            button1.Enabled = !privateActiveDirectorycheckBox.Visible;
            button2.Enabled = !privateActiveDirectorycheckBox.Visible;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // re fill data from controls to the model
            Model = FillModelData();
            // Validate the model
            var validationContext = new ValidationContext(Model, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(Model, validationContext, validationResults, true);

            var validatable = (IValidatableObject) Model;
            if (validatable != null)
                validationResults.AddRange(validatable.Validate(validationContext));

            try
            {
                var context = new ClientContext(Model.Url);
            }
            catch (Exception ex)
            {
                validationResults.Add(new ValidationResult(string.Format("Invalid URL ({0})", ex.Message)));
            }

            // validate the form, if input data is valid => DialogResult.OK
            if (!validationResults.Any())
            {
                DialogResult = DialogResult.OK;
                return;
            }

            // If not => display error message
            var stringBuilder = new StringBuilder();
            var i = 0;
            stringBuilder.AppendLine("Invalid input data");
            foreach (var validationResult in validationResults)
            {
                i++;
                stringBuilder.AppendLine(string.Format("{0}) {1}", i, validationResult.ErrorMessage));
            }
            MessageBox.Show(stringBuilder.ToString(), @"Invalid input data", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {

        }
    }
}

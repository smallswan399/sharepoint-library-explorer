namespace Main
{
    partial class SiteDetailsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SiteDetailsForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.serverNameTextBox = new System.Windows.Forms.TextBox();
            this.serverUrlTextBox = new System.Windows.Forms.TextBox();
            this.serverDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.enableServerCheckBox = new System.Windows.Forms.CheckBox();
            this.authenticationGroupBox = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.privateActiveDirectorycheckBox = new System.Windows.Forms.CheckBox();
            this.rePasswordTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.domainTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.authenticationOptionComboBox = new System.Windows.Forms.ComboBox();
            this.requireAutheticationCheckBox = new System.Windows.Forms.CheckBox();
            this.testButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.serverVersionComboBox = new System.Windows.Forms.ComboBox();
            this.showHidenItemsChk = new System.Windows.Forms.CheckBox();
            this.loadSubSitesCheckBox = new System.Windows.Forms.CheckBox();
            this.authenticationGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F);
            this.label1.Location = new System.Drawing.Point(40, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server Name *";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F);
            this.label2.Location = new System.Drawing.Point(49, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Server URL *";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F);
            this.label3.Location = new System.Drawing.Point(18, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Server Description";
            // 
            // serverNameTextBox
            // 
            this.serverNameTextBox.Font = new System.Drawing.Font("Arial", 9.75F);
            this.serverNameTextBox.Location = new System.Drawing.Point(138, 17);
            this.serverNameTextBox.Name = "serverNameTextBox";
            this.serverNameTextBox.Size = new System.Drawing.Size(303, 22);
            this.serverNameTextBox.TabIndex = 1;
            // 
            // serverUrlTextBox
            // 
            this.serverUrlTextBox.Font = new System.Drawing.Font("Arial", 9.75F);
            this.serverUrlTextBox.Location = new System.Drawing.Point(138, 53);
            this.serverUrlTextBox.Name = "serverUrlTextBox";
            this.serverUrlTextBox.Size = new System.Drawing.Size(303, 22);
            this.serverUrlTextBox.TabIndex = 3;
            // 
            // serverDescriptionTextBox
            // 
            this.serverDescriptionTextBox.Font = new System.Drawing.Font("Arial", 9.75F);
            this.serverDescriptionTextBox.Location = new System.Drawing.Point(138, 89);
            this.serverDescriptionTextBox.Multiline = true;
            this.serverDescriptionTextBox.Name = "serverDescriptionTextBox";
            this.serverDescriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.serverDescriptionTextBox.Size = new System.Drawing.Size(303, 83);
            this.serverDescriptionTextBox.TabIndex = 5;
            // 
            // enableServerCheckBox
            // 
            this.enableServerCheckBox.AutoSize = true;
            this.enableServerCheckBox.Checked = true;
            this.enableServerCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableServerCheckBox.Font = new System.Drawing.Font("Arial", 9.75F);
            this.enableServerCheckBox.Location = new System.Drawing.Point(18, 224);
            this.enableServerCheckBox.Name = "enableServerCheckBox";
            this.enableServerCheckBox.Size = new System.Drawing.Size(107, 20);
            this.enableServerCheckBox.TabIndex = 8;
            this.enableServerCheckBox.Text = "Enable Server";
            this.enableServerCheckBox.UseVisualStyleBackColor = true;
            // 
            // authenticationGroupBox
            // 
            this.authenticationGroupBox.Controls.Add(this.button2);
            this.authenticationGroupBox.Controls.Add(this.button1);
            this.authenticationGroupBox.Controls.Add(this.privateActiveDirectorycheckBox);
            this.authenticationGroupBox.Controls.Add(this.rePasswordTextBox);
            this.authenticationGroupBox.Controls.Add(this.label8);
            this.authenticationGroupBox.Controls.Add(this.passwordTextBox);
            this.authenticationGroupBox.Controls.Add(this.usernameTextBox);
            this.authenticationGroupBox.Controls.Add(this.domainTextBox);
            this.authenticationGroupBox.Controls.Add(this.label7);
            this.authenticationGroupBox.Controls.Add(this.label6);
            this.authenticationGroupBox.Controls.Add(this.label5);
            this.authenticationGroupBox.Controls.Add(this.label4);
            this.authenticationGroupBox.Controls.Add(this.authenticationOptionComboBox);
            this.authenticationGroupBox.Font = new System.Drawing.Font("Arial", 9.75F);
            this.authenticationGroupBox.Location = new System.Drawing.Point(18, 292);
            this.authenticationGroupBox.Name = "authenticationGroupBox";
            this.authenticationGroupBox.Size = new System.Drawing.Size(423, 227);
            this.authenticationGroupBox.TabIndex = 10;
            this.authenticationGroupBox.TabStop = false;
            this.authenticationGroupBox.Text = "Authentication";
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.Black;
            this.button2.Location = new System.Drawing.Point(372, 97);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(26, 20);
            this.button2.TabIndex = 7;
            this.button2.Text = "...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(372, 61);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(26, 20);
            this.button1.TabIndex = 4;
            this.button1.Text = "...";
            this.button1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // privateActiveDirectorycheckBox
            // 
            this.privateActiveDirectorycheckBox.AutoSize = true;
            this.privateActiveDirectorycheckBox.Font = new System.Drawing.Font("Arial", 9.75F);
            this.privateActiveDirectorycheckBox.Location = new System.Drawing.Point(156, 199);
            this.privateActiveDirectorycheckBox.Name = "privateActiveDirectorycheckBox";
            this.privateActiveDirectorycheckBox.Size = new System.Drawing.Size(160, 20);
            this.privateActiveDirectorycheckBox.TabIndex = 12;
            this.privateActiveDirectorycheckBox.Text = "Private Active Directory";
            this.privateActiveDirectorycheckBox.UseVisualStyleBackColor = true;
            this.privateActiveDirectorycheckBox.Visible = false;
            // 
            // rePasswordTextBox
            // 
            this.rePasswordTextBox.Location = new System.Drawing.Point(156, 169);
            this.rePasswordTextBox.Name = "rePasswordTextBox";
            this.rePasswordTextBox.PasswordChar = '*';
            this.rePasswordTextBox.Size = new System.Drawing.Size(206, 22);
            this.rePasswordTextBox.TabIndex = 11;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 9.75F);
            this.label8.Location = new System.Drawing.Point(28, 172);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(110, 16);
            this.label8.TabIndex = 10;
            this.label8.Text = "Retype Password";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(156, 133);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Size = new System.Drawing.Size(206, 22);
            this.passwordTextBox.TabIndex = 9;
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Location = new System.Drawing.Point(156, 97);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(206, 22);
            this.usernameTextBox.TabIndex = 6;
            // 
            // domainTextBox
            // 
            this.domainTextBox.Location = new System.Drawing.Point(156, 61);
            this.domainTextBox.Name = "domainTextBox";
            this.domainTextBox.Size = new System.Drawing.Size(206, 22);
            this.domainTextBox.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 9.75F);
            this.label7.Location = new System.Drawing.Point(73, 136);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 16);
            this.label7.TabIndex = 8;
            this.label7.Text = "Password";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 9.75F);
            this.label6.Location = new System.Drawing.Point(71, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 16);
            this.label6.TabIndex = 5;
            this.label6.Text = "Username";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F);
            this.label5.Location = new System.Drawing.Point(86, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 16);
            this.label5.TabIndex = 2;
            this.label5.Text = "Domain";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9.75F);
            this.label4.Location = new System.Drawing.Point(16, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Authentication Type";
            // 
            // authenticationOptionComboBox
            // 
            this.authenticationOptionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.authenticationOptionComboBox.FormattingEnabled = true;
            this.authenticationOptionComboBox.Location = new System.Drawing.Point(156, 23);
            this.authenticationOptionComboBox.Name = "authenticationOptionComboBox";
            this.authenticationOptionComboBox.Size = new System.Drawing.Size(206, 24);
            this.authenticationOptionComboBox.TabIndex = 1;
            this.authenticationOptionComboBox.SelectedIndexChanged += new System.EventHandler(this.authenticationOptionComboBox_SelectedIndexChanged);
            // 
            // requireAutheticationCheckBox
            // 
            this.requireAutheticationCheckBox.AutoSize = true;
            this.requireAutheticationCheckBox.Checked = true;
            this.requireAutheticationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.requireAutheticationCheckBox.Enabled = false;
            this.requireAutheticationCheckBox.Font = new System.Drawing.Font("Arial", 9.75F);
            this.requireAutheticationCheckBox.Location = new System.Drawing.Point(139, 224);
            this.requireAutheticationCheckBox.Name = "requireAutheticationCheckBox";
            this.requireAutheticationCheckBox.Size = new System.Drawing.Size(157, 20);
            this.requireAutheticationCheckBox.TabIndex = 9;
            this.requireAutheticationCheckBox.Text = "Require Authentication";
            this.requireAutheticationCheckBox.UseVisualStyleBackColor = true;
            this.requireAutheticationCheckBox.CheckedChanged += new System.EventHandler(this.requireAutheticationCheckBox_CheckedChanged);
            // 
            // testButton
            // 
            this.testButton.Font = new System.Drawing.Font("Arial", 9.75F);
            this.testButton.Location = new System.Drawing.Point(18, 541);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(116, 27);
            this.testButton.TabIndex = 11;
            this.testButton.Text = "Test Connection";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Font = new System.Drawing.Font("Arial", 9.75F);
            this.cancelButton.Location = new System.Drawing.Point(347, 541);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(94, 27);
            this.cancelButton.TabIndex = 13;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Font = new System.Drawing.Font("Arial", 9.75F);
            this.okButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.okButton.Location = new System.Drawing.Point(236, 541);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(94, 27);
            this.okButton.TabIndex = 12;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 9.75F);
            this.label9.Location = new System.Drawing.Point(43, 189);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 16);
            this.label9.TabIndex = 6;
            this.label9.Text = "Server version";
            // 
            // serverVersionComboBox
            // 
            this.serverVersionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.serverVersionComboBox.Font = new System.Drawing.Font("Arial", 9.75F);
            this.serverVersionComboBox.FormattingEnabled = true;
            this.serverVersionComboBox.Location = new System.Drawing.Point(138, 186);
            this.serverVersionComboBox.Name = "serverVersionComboBox";
            this.serverVersionComboBox.Size = new System.Drawing.Size(303, 24);
            this.serverVersionComboBox.TabIndex = 7;
            // 
            // showHidenItemsChk
            // 
            this.showHidenItemsChk.AutoSize = true;
            this.showHidenItemsChk.Enabled = false;
            this.showHidenItemsChk.Font = new System.Drawing.Font("Arial", 9.75F);
            this.showHidenItemsChk.Location = new System.Drawing.Point(311, 224);
            this.showHidenItemsChk.Name = "showHidenItemsChk";
            this.showHidenItemsChk.Size = new System.Drawing.Size(130, 20);
            this.showHidenItemsChk.TabIndex = 14;
            this.showHidenItemsChk.Text = "Show hiden items";
            this.showHidenItemsChk.UseVisualStyleBackColor = true;
            // 
            // loadSubSitesCheckBox
            // 
            this.loadSubSitesCheckBox.AutoSize = true;
            this.loadSubSitesCheckBox.Font = new System.Drawing.Font("Arial", 9.75F);
            this.loadSubSitesCheckBox.Location = new System.Drawing.Point(18, 250);
            this.loadSubSitesCheckBox.Name = "loadSubSitesCheckBox";
            this.loadSubSitesCheckBox.Size = new System.Drawing.Size(112, 20);
            this.loadSubSitesCheckBox.TabIndex = 15;
            this.loadSubSitesCheckBox.Text = "Load sub sites";
            this.loadSubSitesCheckBox.UseVisualStyleBackColor = true;
            // 
            // SiteDetailsForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(458, 580);
            this.Controls.Add(this.loadSubSitesCheckBox);
            this.Controls.Add(this.showHidenItemsChk);
            this.Controls.Add(this.serverVersionComboBox);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.testButton);
            this.Controls.Add(this.requireAutheticationCheckBox);
            this.Controls.Add(this.authenticationGroupBox);
            this.Controls.Add(this.enableServerCheckBox);
            this.Controls.Add(this.serverDescriptionTextBox);
            this.Controls.Add(this.serverUrlTextBox);
            this.Controls.Add(this.serverNameTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SiteDetailsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add SharePoint Site";
            this.Load += new System.EventHandler(this.AddSite_Load);
            this.authenticationGroupBox.ResumeLayout(false);
            this.authenticationGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox serverNameTextBox;
        private System.Windows.Forms.TextBox serverUrlTextBox;
        private System.Windows.Forms.TextBox serverDescriptionTextBox;
        private System.Windows.Forms.CheckBox enableServerCheckBox;
        private System.Windows.Forms.GroupBox authenticationGroupBox;
        private System.Windows.Forms.CheckBox requireAutheticationCheckBox;
        private System.Windows.Forms.Button testButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox rePasswordTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.TextBox domainTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox authenticationOptionComboBox;
        private System.Windows.Forms.CheckBox privateActiveDirectorycheckBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox serverVersionComboBox;
        private System.Windows.Forms.CheckBox showHidenItemsChk;
        private System.Windows.Forms.CheckBox loadSubSitesCheckBox;
    }
}
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Main
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;
            label1.Text = $"Litera SharePoint Integration - v{version}";
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            //DialogHelper.ShowErrorDialog(new Exception("this is a test exception", new Exception("this inner exception")), @"C:\Users\Administrator\Desktop\80842029_738090008.js");
            Close();
        }
    }
}

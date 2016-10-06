using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChangeProMockup
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var handle = this.Handle;
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo(@"C:\Program Files (x86)\Litera\SharePoint\lcp_dmsst.exe", "loadfromdms -h " + handle)
            };
            process.Start();
            process.WaitForExit();
            MessageBox.Show("Run");
        }
    }
}

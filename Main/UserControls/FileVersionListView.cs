using System.Windows.Forms;
using Main.Services;
using Main.Services.Domains;

namespace Main.UserControls
{
    public class FileVersionListView : ListView
    {
        // public SharePointFileVersion SharePointFileVersion { get; set; }
        // public SharePointFile SharePointFile { get; set; }
        public ISharePointService SharePointService { get; set; }
    }
}
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Libs;
using Main.Repos;
using Main.Services;
using Main.Services.Domains;

namespace Main.UserControls
{
    public class FilesListView : ListView
    {
        public IRepoProvider RepoProvider { get; set; }

        /// <summary>
        /// FilesListView related to a SiteNodeTreeNode
        /// </summary>
        public SiteNodeTreeNode SiteNodeTreeNode { get; set; }

        public ISharePointService SharePointService { get; set; }
        /// <summary>
        /// FilesListView is as a ISharePointFileContainer (SharePointLibrary or SharePointFolder)
        /// </summary>
        public ISharePointFileContainer SharePointFileContainer { get; set; }

        /// <summary>
        /// Fill files into FilesListView, the files get from selected site node
        /// </summary>
        public void PopulateFilesIntoFilesListView()
        {
            Items.Clear();
            if (SharePointFileContainer != null)
            {
                //(SharePointExpandableObjcect as ISharePointFileContainer).SharePointLibraryItems
                (SharePointFileContainer).SharePointLibraryItems.ToList().ForEach(s =>
                {
                    if (s is SharePointFolder)
                    {
                        Items.Add(new FilesListViewItem(new[] { s.Name, s.CreatedDateTime.ToString(CultureInfo.InvariantCulture), s.Author, "", "", s.LastModifiedDateTime.ToString(CultureInfo.InvariantCulture) }, 1, RepoProvider, SharePointService)
                        {
                            SharePointLibraryItem = s
                        });
                    }
                    var file = s as SharePointFile;
                    if (file != null)
                    {
                        Items.Add(new FilesListViewItem(
                                new[]
                                {
                                    s.Name, s.CreatedDateTime.ToString(CultureInfo.InvariantCulture), file.Author,
                                    ByteSize.FromBytes(s.FileSize).ToString("0.00"),
                                    file.CheckoutUser, s.LastModifiedDateTime.ToString(CultureInfo.InvariantCulture)
                                },
                                GetListItemImageIndex(file.FileExtension), RepoProvider,
                                SharePointService)
                            {
                                SharePointLibraryItem = s
                            });
                    }
                });
            }
        }



        private int GetListItemImageIndex(string extension)
        {
            switch (extension)
            {
                case "docx":
                    return 6;
                case "doc":
                    return 6;
                case "pdf":
                    return 3;
                case "txt":
                    return 4;
                case "gif":
                    return 7;
                case "img":
                    return 7;
                case "jpg":
                    return 7;
                case "png":
                    return 7;
                case "xlsx":
                    return 9;
                case "ppt":
                    return 11;
                case "pptx":
                    return 12;
                case "rtf":
                    return 13;
                case "xls":
                    return 14;
                case "zip":
                    return 5;
                case "rar":
                    return 5;
                case "htm":
                    return 2;
                case "xml":
                    return 8;
                case "pptm":
                    return 15;
                case "xltm":
                    return 16;
                case "xlsm":
                    return 17;
                case "pps":
                    return 18;
                case "ppsm":
                    return 19;
                case "ppsx":
                    return 20;
                case "docm":
                    return 21;
                case "xlsb":
                    return 22;
                default:
                    return 0;
            }
        }
    }
}

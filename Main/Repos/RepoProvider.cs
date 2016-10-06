using System.IO;
using System.Xml.Serialization;
using Entities;

namespace Main.Repos
{
    public enum ResultMode
    {
        
        SelectFiles,
        DownloadFiles, 
        UploadFiles, 
        None
    }
    public class RepoProvider : IRepoProvider
    {
        private readonly XmlSerializer xmlSerializer = new XmlSerializer(typeof(Data));
        private readonly string dataFileName;
        // private const string DataFileNameDebug = "data_debug.xml";
        private readonly string dir;
        private static Data data;
        public void Dispose()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dir">The folder that contain data.xml. If is empty, the current directory will be used</param>
        public RepoProvider(string dir)
        {
#if DEBUG
            dataFileName = "data_debug.xml";
#else
            dataFileName = "data.xml";
#endif
            if (string.IsNullOrWhiteSpace(dir))
            {
                dir = Directory.GetCurrentDirectory();
            }
            this.dir = dir;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(Path.Combine(dir, dataFileName)))
            {
                data = new Data();
                using (var writer = new StreamWriter(Path.Combine(dir, dataFileName)))
                {
                    xmlSerializer.Serialize(writer, data);
                }
            }
            else
            {
                using (var reader = new StreamReader(Path.Combine(dir, dataFileName)))
                {
                    data = xmlSerializer.Deserialize(reader) as Data;
                }
            }
            SiteRepository = new SiteRepository(data);
        }

        public ISiteRepository SiteRepository { get; private set; }

        public SelectedFiles SelectedFiles
        {
            get { return data.SelectedFiles; }
            set { data.SelectedFiles = value; }
        }

        public DownloadedFiles DownloadedFiles
        {
            get { return data.DownloadedFiles; }
            set { data.DownloadedFiles = value; }
        }

        public UploadingFiles UploadingFiles
        {
            get { return data.UploadingFiles; }
            set { data.UploadingFiles = value; }
        }
        public UploadedFiles UploadedFiles
        {
            get { return data.UploadedFiles; }
            set { data.UploadedFiles = value; }
        }

        public SelectedFileVersions SelectedFileVersions
        {
            get { return data.SelectedFileVersions; }
            set { data.SelectedFileVersions = value; }
        }

        public string DownloadDirectory
        {
            get { return data.DownloadDirectory; }
            set { data.DownloadDirectory = value; }
        }

        public int SaveChanges(ResultMode mode = ResultMode.None)
        {
            using (var writer = new StreamWriter(Path.Combine(dir, dataFileName)))
            {
                xmlSerializer.Serialize(writer, data);
            }
            return 0;
        }
    }
}
using Core.Libs;
using Main.Libs;

namespace Main.Services.Domains
{
    public abstract class SharePointObject : ISharePointObject
    {
        public string Url { get; set; }
        public string RelativeUrl { get; set; }

        public string RootUrl
        {
            get { return Utils.GetRootUrl(Url, RelativeUrl); }
        }

        public virtual string User { get; set; }
    }
}
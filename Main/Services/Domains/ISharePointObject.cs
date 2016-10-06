namespace Main.Services.Domains
{
    public interface ISharePointObject
    {
        string RelativeUrl { get; set; }
        //string URL { get; set; }
        string RootUrl { get; }

        string Url { get; set; }

        /// <summary>
        /// The current user that was login
        /// </summary>
        string User { get; }
    }
}

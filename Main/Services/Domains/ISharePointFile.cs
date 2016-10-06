namespace Main.Services.Domains
{
    public interface ISharePointFile : ISharePointLibraryItem
    {
        string FileExtension { get; set; }
        void Download();
        SharePointLibrary GetAncestorSharePointLibrary();
        //bool NeedToCheckout();
    }
}
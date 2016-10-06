using System;

namespace Main.Services.Domains
{
    public interface ISharePointSiteItem : ISharePointObject
    {
        SharePointSite ParentSharePointSite { get; set; }
        Guid Id { get; set; }
    }
}

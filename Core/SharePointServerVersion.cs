using System.ComponentModel;

namespace Core
{
    public enum SharePointServerVersion
    {
        [Description("SharePoint 2007")]
        SharePoint2007,
        [Description("SharePoint 2010")]
        SharePoint2010,
        [Description("SharePoint 2013")]
        SharePoint2013,
        [Description("SharePoint Online")]
        SharePointOnline,
    }
}
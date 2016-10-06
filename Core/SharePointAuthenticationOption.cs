using System.ComponentModel;

namespace Core
{
    public enum SharePointAuthenticationOption
    {
        [Description("Normal Authentication")]
        RequireNormalAuthentication,
        [Description("Forms Authentication")]
        RequireFormsAuthentication,
        [Description("Active Directory")]
        ActiveDirectory
    }
}
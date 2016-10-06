using Core;
using Core.Libs;

namespace Entities
{
    public class Credential
    {
        public string Username { get; set; }
        public string Password { get; set; }

        //[XmlIgnore]
        public string ReTypePassword { get; set; }
        public string DomainName { get; set; }
        public SharePointAuthenticationOption SharePointAuthenticationOption { get; set; }

        // Only for Office 365 Active directory
        public bool PrivateActiveDirectory { get; set; }

        public ClearTextCredential ToClearTextCredential()
        {
            return new ClearTextCredential()
            {
                DomainName = DomainName,
                Username = Username,
                Password = Password.DecryptStringAES(Constants.SceretString),
                PrivateActiveDirectory = PrivateActiveDirectory,
                ReTypePassword = ReTypePassword.DecryptStringAES(Constants.SceretString),
                SharePointAuthenticationOption = SharePointAuthenticationOption
            };
        }
    }
}

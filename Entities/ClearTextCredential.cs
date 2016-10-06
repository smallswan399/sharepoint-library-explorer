using Core;
using Core.Libs;

namespace Entities
{
    public class ClearTextCredential
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ReTypePassword { get; set; }
        public string DomainName { get; set; }
        public SharePointAuthenticationOption SharePointAuthenticationOption { get; set; }
        public bool PrivateActiveDirectory { get; set; }

        public Credential ToCredential(Credential credential = null)
        {
            if (credential == null)
            {
                return new Credential()
                {
                    DomainName = DomainName,
                    Username = Username,
                    Password = Password.EncryptStringAES(Constants.SceretString),
                    PrivateActiveDirectory = PrivateActiveDirectory,
                    ReTypePassword = ReTypePassword.EncryptStringAES(Constants.SceretString),
                    SharePointAuthenticationOption = SharePointAuthenticationOption
                };
            }
            credential.DomainName = DomainName;
            credential.Username = Username;
            credential.Password = Password.EncryptStringAES(Constants.SceretString);
            credential.PrivateActiveDirectory = PrivateActiveDirectory;
            credential.ReTypePassword = ReTypePassword.EncryptStringAES(Constants.SceretString);
            credential.SharePointAuthenticationOption = SharePointAuthenticationOption;
            return credential;
        }
    }
}

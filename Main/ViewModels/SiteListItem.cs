using System;

namespace Main.ViewModels
{
    public class SiteListItem
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Enable { get; set; }
        public string Description { get; set; }
        public string CredentialUsername { get; set; }
        public string Domain { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Core;

namespace Entities
{
    public class Site : IValidatableObject
    {
        [XmlElement(ElementName = "Guid")]
        public Guid Id { get; set; }
        [Required]
        public string Url { get; set; }
        /// <summary>
        /// First level parent site Url
        /// </summary>
        public string RootUrl { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Enable { get; set; }
        public SharePointServerVersion SharePointServerVersion { get; set; }
        public Credential Credential { get; set; }
        public bool RequireAuthentication { get; set; }
        [XmlIgnore]
        public IEnumerable<Site> SubSites { get; set; }
        public bool IncludeSubSites { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();
            if (!RequireAuthentication || Credential.SharePointAuthenticationOption == SharePointAuthenticationOption.ActiveDirectory)
                return result;
            if (Credential == null)
            {
                result.Add(new ValidationResult("The User name, Password and Retype Password field are required."));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Credential.Username))
                {
                    result.Add(new ValidationResult("The User name field is required."));
                }
                if (string.IsNullOrWhiteSpace(Credential.Password))
                {
                    result.Add(new ValidationResult("The Password field is required."));
                }
                if (string.IsNullOrWhiteSpace(Credential.ReTypePassword))
                {
                    result.Add(new ValidationResult("The Retype Password field is required."));
                }
            }
            return result;
        }
    }
}

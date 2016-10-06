using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core;
using Entities;
using Main.Services.Domains;

namespace Main.ViewModels
{
    public class SiteDetails : IValidatableObject
    {
        public Guid Id { get; set; }
        [Required]
        [Display(Name = @"Server URL")]
        public string Url { get; set; }
        [Required]
        [Display(Name = @"Server Name")]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Enable { get; set; }
        public bool IncludeSubSites { get; set; }
        public SharePointServerVersion SharePointServerVersion { get; set; }
        public ClearTextCredential Credential { get; set; }
        public bool RequireAuthentication { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();
            if (!RequireAuthentication || Credential.SharePointAuthenticationOption == SharePointAuthenticationOption.ActiveDirectory)
                return result;
            if (Credential == null)
            {
                result.Add(new ValidationResult("The Username, Password and Retype Password field are required."));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Credential.Username))
                {
                    result.Add(new ValidationResult("The Username field is required."));
                }
                if (string.IsNullOrWhiteSpace(Credential.Password))
                {
                    result.Add(new ValidationResult("The Password field is required."));
                }
                if (string.IsNullOrWhiteSpace(Credential.ReTypePassword))
                {
                    result.Add(new ValidationResult("The Retype Password field is required."));
                }
                if (Credential.Password != Credential.ReTypePassword)
                {
                    result.Add(new ValidationResult("The Password and the Retype Password field must be same.", new List<string> { "Password" }));
                }
            }
            return result;
        }
    }
}

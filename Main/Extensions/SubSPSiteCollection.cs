using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Entities;
using Extensions;

namespace Main.Extensions
{
    class SubSPSiteCollection : IAdditionalSPSite
    {
        [Export(typeof(IAdditionalSPSite))]
        public IEnumerable<Site> GetAddtionalSites(Site baseSite)
        {
            throw new NotImplementedException();
        }
    }
}

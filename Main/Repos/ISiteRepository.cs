using System;
using System.Collections.Generic;
using Entities;

namespace Main.Repos
{
    public interface ISiteRepository : IRepository<Site>
    {
        IEnumerable<Site> GetEnableSites();

        Site GetByIdRecursive(Guid id);
    }
}
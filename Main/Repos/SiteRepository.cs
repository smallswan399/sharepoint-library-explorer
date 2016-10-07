using System;
using System.Collections.Generic;
using System.Linq;
using Entities;

namespace Main.Repos
{
    public class SiteRepository : ISiteRepository
    {
        private readonly Data data;
        public SiteRepository(Data data)
        {
            this.data = data;
        }

        public IEnumerable<Site> GetAll()
        {
            return data.Sites;
        }

        public IEnumerable<Site> GetEnableSites()
        {
            return data.Sites.Where(s => s.Enable);
        }

        public Site GetByIdRecursive(Guid id)
        {
            var siteL1s = GetById(id);
            if (siteL1s != null)
            {
                return siteL1s;
            }
            var subSite =
                data.Sites.Select(s => s.SubSites)
                    .FirstOrDefault(s => s.Select(t => t.Id).Contains(id));
            return subSite?.FirstOrDefault(s => s.Id == id);
        }

        public Site GetById(Guid id)
        {
            return data.Sites.FirstOrDefault(s => s.Id == id);
        }

        public void Add(Site obj)
        {
            data.Sites.Add(new Site()
            {
                Credential = obj.Credential,
                IncludeSubSites = obj.IncludeSubSites,
                Description = obj.Description,
                Enable = obj.Enable,
                Name = obj.Name,
                Id = Guid.NewGuid(),
                Url = obj.Url,
                RequireAuthentication = obj.RequireAuthentication,
                SharePointServerVersion = obj.SharePointServerVersion
            });
        }

        public void Delete(Site obj)
        {
            //var allItems = GetAll();
            var item = data.Sites.FirstOrDefault(s => s.Id == obj.Id);
            if (item != null)
            {
                data.Sites.Remove(item);
            }
        }

        public void Delete(Guid id)
        {
            Delete(new Site(){Id = id});
        }
    }
}
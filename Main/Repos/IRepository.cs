using System;
using System.Collections.Generic;

namespace Main.Repos
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(Guid id);
        void Add(T obj);
        void Delete(T obj);
        void Delete(Guid id);
    }
}

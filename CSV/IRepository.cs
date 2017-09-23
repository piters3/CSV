using System;
using System.Collections.Generic;

namespace CSV
{
    interface IRepository<T> : IDisposable
    {
        IEnumerable<T> GetAll();
        T Get(int id);
        void Add(T newEntity);
        void Update(int id, T updatedEntity);
        void Delete(T entity);
        int Commit();
    }
}

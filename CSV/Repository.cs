using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace CSV
{
    class Repository<T> : IRepository<T> where T : class
    {
        DbContext _ctx;
        DbSet<T> _set;

        public Repository(DbContext ctx)
        {
            _ctx = ctx;
            _set = _ctx.Set<T>();
        }

        public void Add(T newEntity)
        {
            _set.Add(newEntity);
        }

        public void Delete(T entity)
        {
            _set.Remove(entity);
        }

        public T Get(int id)
        {
            return _set.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return _set;
        }

        public int Commit()
        {
            return _ctx.SaveChanges();
        }

        public void Update(int Id, T updatedEntity)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _ctx.Dispose();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Prawnbot.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Prawnbot.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbContext context;

        public Repository(IUnitOfWork unitOfWork)
        {
            context = unitOfWork.CurrentContext();
        }

        public IEnumerable<T> GetAll()
        {
            return context.Set<T>().AsEnumerable();
        }

        public IQueryable<T> GetQuery()
        {
            return context.Set<T>().AsQueryable();
        }

        public T GetById(int id)
        {
            return context.Set<T>().Find(id);
        }

        public Task<T> GetByIdAsync(int id, CancellationToken token)
        {
            return context.Set<T>().FindAsync(token, id);
        }

        public IQueryable<T> FilterBy(Expression<Func<T, bool>> predicate)
        {
            return context.Set<T>().Where<T>(predicate);
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            EntityEntry<T> entry = context.Entry(entity);
            if (entry.State != EntityState.Detached)
            {
                context.Set<T>().Remove(entity);
            }
        }

        public void DeleteAttachFirst(T entity)
        {
            context.Set<T>().Attach(entity);
            context.Entry(entity).State = EntityState.Deleted;
        }

        public void Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            context.Set<T>().Add(entity);
        }

        public virtual void Update(T entity)
        {
            context.Set<T>().Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public EntityEntry<T> DbEntityEntry(T entry)
        {
            return context.Entry(entry);
        }
    }
}

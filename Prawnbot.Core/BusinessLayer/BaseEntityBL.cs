using Microsoft.EntityFrameworkCore;
using Prawnbot.Core.Interfaces;
using Prawnbot.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Prawnbot.Core.BusinessLayer
{
    public class BaseEntityBL<T> : IBaseEntityBL<T> where T : class
    {
        public readonly BotDatabaseContext Context;

        public BaseEntityBL(BotDatabaseContext context)
        {
            this.Context = context;
        }

        public void Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Context.Set<T>().Remove(entity);
        }

        public IQueryable<T> GetAll()
        {
            return Context.Set<T>().AsQueryable();
        }

        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Context.Set<T>().Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public T GetById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return Context.Set<T>().Find(id);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return await Context.Set<T>().FindAsync(id);
        }
    }
}

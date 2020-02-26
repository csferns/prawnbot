using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Prawnbot.Data.Interfaces
{
    public interface IRepository<T> : IRepositoryBase where T : class
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetQuery();
        T GetById(int id);
        Task<T> GetByIdAsync(int id, CancellationToken token);
        IQueryable<T> FilterBy(Expression<Func<T, bool>> predicate);
        void Delete(T entity);
        void DeleteAttachFirst(T entity);
        void Add(T entity);
        void Update(T entity);
        EntityEntry<T> DbEntityEntry(T entry);
    }
}

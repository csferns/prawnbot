using System.Linq;

namespace Prawnbot.Core.Database.Interfaces
{
    /// <summary>
    /// Base Business Layer for Business layers using entities
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseEntityBL<T> where T : class
    {
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
        IQueryable<T> GetAll();
        T GetById(int id);
    }
}

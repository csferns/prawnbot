using Prawnbot.Core.Interfaces;
using Prawnbot.Data.Interfaces;
using System.Linq;

namespace Prawnbot.Core.BusinessLayer
{
    public class BaseEntityBL<T> : IBaseEntityBL<T> where T : class
    {
        private readonly IRepository<T> repository;
        private readonly IUnitOfWork unitOfWork;
        public BaseEntityBL(IUnitOfWork unitOfWork, IRepository<T> repository)
        {
            this.unitOfWork = unitOfWork;
            this.repository = repository;
        }

        public void Add(T entity)
        {
            repository.Add(entity);
        }

        public void Delete(T entity)
        {
            repository.Delete(entity);
        }

        public IQueryable<T> GetAll()
        {
            return repository.GetAll();
        }

        public void Update(T entity)
        {
            repository.Update(entity);
        }

        public T GetById(int id)
        {
            return repository.GetById(id);
        }
    }
}

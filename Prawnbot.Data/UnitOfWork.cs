using Microsoft.EntityFrameworkCore;
using Prawnbot.Data.Interfaces;
using System;

namespace Prawnbot.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private DbContext context;

        public UnitOfWork(DbContext context)
        {
            this.context = context;
        }

        public IRepository<T> CreateRepository<T>() where T : class
        {
            return new Repository<T>(this);
        }

        public void Dispose()
        {
            if (this.context != null)
            {
                try
                {
                    this.context.Dispose();
                }
                catch
                {
                    
                }
            }
            GC.SuppressFinalize(this);
        }

        public void Commit()
        {
            context.SaveChanges();
        }

        public DbContext CurrentContext()
        {
            return context;
        }
    }
}

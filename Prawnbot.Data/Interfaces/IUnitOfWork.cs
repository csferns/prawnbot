using Microsoft.EntityFrameworkCore;
using System;

namespace Prawnbot.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        DbContext CurrentContext();
        IRepository<T> CreateRepository<T>() where T : class;
        void Commit();
    }
}

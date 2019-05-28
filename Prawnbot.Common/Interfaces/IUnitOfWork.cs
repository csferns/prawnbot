using System;
using System.Collections.Generic;
using System.Text;

namespace Prawnbot.Common.Interfaces
{
    public interface IUnitOfWork
    {
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}

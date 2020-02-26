using Prawnbot.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prawnbot.Core.Interfaces
{
    public interface IBaseService
    {
        Response<TEntity> LoadResponse<TEntity>(TEntity item);
        ListResponse<TEntity> LoadListResponse<TEntity>(IEnumerable<TEntity> list);
    }
}

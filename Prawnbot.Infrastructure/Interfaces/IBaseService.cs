using Prawnbot.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prawnbot.Infrastructure.Interfaces
{
    public interface IBaseService
    {
        Response<TEntity> LoadResponse<TEntity>(TEntity item);
        Response<TEntity> LoadResponse<TEntity>(Func<TEntity> func);
        ListResponse<TEntity> LoadListResponse<TEntity>(IEnumerable<TEntity> list);
        ResponseBase LoadResponseBase(Action action);
        Task<ResponseBase> LoadResponseBaseAsync(Action action);
    }
}

using Prawnbot.Infrastructure;
using System.Collections.Generic;

namespace Prawnbot.Core.ServiceLayer
{
    public class BaseService
    {
        protected Response<TEntity> LoadResponse<TEntity>(TEntity item)
        {
            Response<TEntity> response = new Response<TEntity>
            {
                Entity = item
            };

            return response;
        }

        protected ListResponse<TEntity> LoadListResponse<TEntity>(IEnumerable<TEntity> list)
        {
            ListResponse<TEntity> response = new ListResponse<TEntity>
            {
                Entities = list
            };

            return response;
        }
    }
}

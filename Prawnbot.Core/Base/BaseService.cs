using Prawnbot.Core.Framework;
using System.Collections.Generic;

namespace Prawnbot.Core.Base
{
    public class BaseService
    {
        public BaseService()
        {
            
        }

        protected Response<TEntity> LoadResponse<TEntity>(TEntity item)
        {
            Response<TEntity> response = new Response<TEntity>
            {
                Entity = item
            };

            return response;
        }

        protected ListResponse<TEntity> LoadListResponse<TEntity>(List<TEntity> list)
        {
            ListResponse<TEntity> response = new ListResponse<TEntity>
            {
                Entities = list
            };

            return response;
        }
    }
}

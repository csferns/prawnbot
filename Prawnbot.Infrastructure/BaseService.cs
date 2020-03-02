using Prawnbot.Infrastructure;
using Prawnbot.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prawnbot.Infrastructure
{
    public class BaseService : IBaseService
    {
        public Response<TEntity> LoadResponse<TEntity>(Func<TEntity> func)
        {
            Response<TEntity> response = new Response<TEntity>();

            try
            {
                TEntity entity = func.Invoke();
                response.Entity = entity;
            }
            catch (Exception e)
            {
                response.SetException("An error occured during LoadResponse<" + typeof(TEntity).Name + ">", e);
            }

            return response;
        }

        public Response<TEntity> LoadResponse<TEntity>(TEntity item)
        {
            Response<TEntity> response = new Response<TEntity>
            {
                Entity = item
            };

            return response;
        }

        public ListResponse<TEntity> LoadListResponse<TEntity>(IEnumerable<TEntity> list)
        {
            ListResponse<TEntity> response = new ListResponse<TEntity>
            {
                Entities = list
            };

            return response;
        }

        public ResponseBase LoadResponseBase(Action action)
        {
            ResponseBase response = new ResponseBase();

            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                response.SetException("An error occured during LoadResponseBaseAsync", e);
            }

            return response;
        }

        public async Task<ResponseBase> LoadResponseBaseAsync(Action action)
        {
            ResponseBase response = new ResponseBase();

            try
            {
                await Task.Run(action);
            }
            catch (Exception e)
            {
                response.SetException("An error occured during LoadResponseBaseAsync", e);
            }

            return response;
        }
    }
}

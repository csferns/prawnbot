using Prawnbot.Core.Interfaces;
using Prawnbot.Core.Model.DTOs;
using Prawnbot.Infrastructure;

namespace Prawnbot.Core.ServiceLayer
{
    public class AlarmService : BaseService, IAlarmService
    {
        private readonly IAlarmBL alarmBL;
        public AlarmService(IAlarmBL alarmBL)
        {
            this.alarmBL = alarmBL;
        }

        public ListResponse<AlarmDTO> GetAll()
        {
            return LoadListResponse(alarmBL.GetAll());
        }

        public Response<AlarmDTO> GetById(int alarmId)
        {
            return LoadResponse(alarmBL.GetById(alarmId));
        }
    }
}

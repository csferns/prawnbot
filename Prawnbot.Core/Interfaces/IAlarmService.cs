using Prawnbot.Core.Model.DTOs;
using Prawnbot.Infrastructure;

namespace Prawnbot.Core.Interfaces
{
    public interface IAlarmService
    {
        ListResponse<AlarmDTO> GetAll();
        Response<AlarmDTO> GetById(int alarmId);
    }
}

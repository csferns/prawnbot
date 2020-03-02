using Prawnbot.Common.DTOs;
using Prawnbot.Infrastructure;

namespace Prawnbot.Core.Database.Interfaces
{
    public interface IAlarmService
    {
        ListResponse<AlarmDTO> GetAll();
        Response<AlarmDTO> GetById(int alarmId);
    }
}

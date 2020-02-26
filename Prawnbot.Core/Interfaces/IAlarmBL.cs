using Prawnbot.Core.Model.DTOs;
using System.Collections.Generic;

namespace Prawnbot.Core.Interfaces
{
    public interface IAlarmBL
    {
        IEnumerable<AlarmDTO> GetAll();
        AlarmDTO GetById(int alarmId);
    }
}

using Prawnbot.Common.DTOs;
using System.Collections.Generic;

namespace Prawnbot.Core.Database.Interfaces
{
    public interface IAlarmBL
    {
        IEnumerable<AlarmDTO> GetAll();
        AlarmDTO GetById(int alarmId);
    }
}

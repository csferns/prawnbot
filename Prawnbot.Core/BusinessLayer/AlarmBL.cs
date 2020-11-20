using AutoMapper;
using AutoMapper.QueryableExtensions;
using Prawnbot.Core.Interfaces;
using Prawnbot.Core.Model.DTOs;
using Prawnbot.Data;
using Prawnbot.Data.Entities;
using System.Collections.Generic;

namespace Prawnbot.Core.BusinessLayer
{
    public class AlarmBL : BaseEntityBL<Alarm>, IAlarmBL
    {
        public AlarmBL(BotDatabaseContext context) : base(context)
        {

        }

        public IEnumerable<AlarmDTO> GetAll()
        {
            return base.GetAll().ProjectTo<AlarmDTO>();
        }

        public AlarmDTO GetById(int alarmId)
        {
            Alarm alarm = base.GetById(alarmId);
            return Mapper.Map<AlarmDTO>(alarm);
        }
    }
}

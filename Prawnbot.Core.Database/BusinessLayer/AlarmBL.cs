using AutoMapper;
using AutoMapper.QueryableExtensions;
using Prawnbot.Common.DTOs;
using Prawnbot.Core.Database.Interfaces;
using Prawnbot.Core.Repository;
using Prawnbot.Data.Entities;
using Prawnbot.Data.Interfaces;
using System.Collections.Generic;

namespace Prawnbot.Core.Database.BusinessLayer
{
    public class AlarmBL : BaseEntityBL<Alarm>, IAlarmBL
    {
        public AlarmBL(IUnitOfWork unitOfWork, IAlarmRepository repository) : base(unitOfWork, repository)
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

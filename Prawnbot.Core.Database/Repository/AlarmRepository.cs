using Prawnbot.Data;
using Prawnbot.Data.Entities;
using Prawnbot.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prawnbot.Core.Repository
{
    public interface IAlarmRepository : IRepository<Alarm>
    {

    }

    public class AlarmRepository : Repository<Alarm>, IAlarmRepository 
    {
        public AlarmRepository(IUnitOfWork unitOfWork) : base (unitOfWork)
        {

        }
    }
}

using Prawnbot.Common.Configuration;
using Prawnbot.Data.Entities;
using System.Data.Entity;

namespace Prawnbot.Data
{
    public class BotDatabaseContext : DbContext
    {
        public BotDatabaseContext() : base(ConfigUtility.DatabaseConnectionString)
        {
        }

        public DbSet<Yotta> Yottas { get; set; }
    }
}

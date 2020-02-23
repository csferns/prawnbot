using Microsoft.EntityFrameworkCore;
using Prawnbot.Common.Configuration;
using Prawnbot.Data.Entities;

namespace Prawnbot.Data
{
    public class BotDatabaseContext : DbContext
    {
        public BotDatabaseContext()
        {

        }

        public DbSet<Yotta> Yottas { get; set; }
        public DbSet<SavedTranslation> Translations { get; set; }
        public DbSet<BotResponse> BotResponse { get; set; }
        public DbSet<Alarm> Alarms { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (ConfigUtility.UseDatabaseConnection)
            {
                options.UseSqlServer(ConfigUtility.DatabaseConnectionString);
            }
            else
            {
                options.UseInMemoryDatabase("BotDatabaseContext");
            }


            base.OnConfiguring(options);
        }
    }
}

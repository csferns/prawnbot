using Microsoft.EntityFrameworkCore;
using Prawnbot.Data.Entities;
using Prawnbot.Utility.Configuration;

namespace Prawnbot.Data
{
    public class BotDatabaseContext : DbContext
    {
        public DbSet<Yotta> Yottas { get; set; }
        public DbSet<SavedTranslation> Translations { get; set; }
        public DbSet<BotResponse> BotResponse { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConfigUtility.DatabaseConnectionString);
        }
    }
}

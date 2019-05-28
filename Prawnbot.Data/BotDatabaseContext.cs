using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Prawnbot.Data.Entities;

namespace Prawnbot.Data
{
    public class BotDatabaseContext : DbContext
    {
        public BotDatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Yotta> Yottas { get; set; }
    }
}

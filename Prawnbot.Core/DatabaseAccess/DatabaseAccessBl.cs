using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Prawnbot.Core.Base;
using Prawnbot.Data;
using System;

namespace Prawnbot.Core.DatabaseAccess
{
    public interface IDatabaseAccessBl
    {

    }

    public class DatabaseAccessBl : BaseBl, IDatabaseAccessBl
    {
        public void Intitialize(IServiceProvider serviceProvider)
        {
            using (var context = new BotDatabaseContext(serviceProvider.GetRequiredService<DbContextOptions<BotDatabaseContext>>()))
            {
                return;
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BotDatabaseContext>(options => options.UseInMemoryDatabase(databaseName: "BigSucc"));
        }
    }
}

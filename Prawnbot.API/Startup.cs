using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Integration.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prawnbot.API.Controllers;
using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Log;
using Prawnbot.Core.ServiceLayer;
using System.Reflection;
using System.Web.Http;

namespace Prawnbot.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            services.AddSingleton<IBotService, BotService>();
            services.AddSingleton<IBotBL, BotBL>();
            services.AddSingleton<ICoreBL, CoreBL>();
            services.AddSingleton<IFileBL, FileBL>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IAPIBL, APIBL>();
            services.AddSingleton<ILogging, Logging>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

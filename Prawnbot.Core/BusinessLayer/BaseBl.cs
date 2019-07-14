using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Prawnbot.Common.Configuration;
using Prawnbot.Core.Framework;
using Prawnbot.Core.Log;
using Prawnbot.Data.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Prawnbot.Core.BusinessLayer
{
    public class BaseBl : Base
    {
        // Constructors
		
        public BaseBl()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("Configuration\\appsettings.json", optional: false);

            ConfigUtility = new ConfigUtility(builder.Build());
        }

        public BaseBl(IUnitOfWork _unitOfWork)
        {
            this._unitOfWork = _unitOfWork;
        }

        public IUnitOfWork _unitOfWork;

        public static ConfigUtility ConfigUtility;

        protected static ILogging logging;

        // Bot properties

        protected readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        protected static DiscordSocketClient _client;
        protected static string _token;
        protected static Discord.Commands.CommandService _commands;
        protected static IServiceProvider _services;
        protected static SocketCommandContext Context;

        // Quartz

        protected static bool IsQuartzInitialized { get; set; }
		
        // Database

        public T GetById<T>(int id) where T : class
        {
            IRepository<T> repository = _unitOfWork.CreateRepository<T>();
            return repository.GetById(id);
        }

        public Task<T> GetByIdAsync<T>(int id) where T : class
        {
            IRepository<T> repository = _unitOfWork.CreateRepository<T>();
            return repository.GetByIdAsync(id, new CancellationToken());
        }

        public void Update<T>(T entity) where T : class
        {
            IRepository<T> repository = _unitOfWork.CreateRepository<T>();
            repository.Update(entity);
        }

        public void Add<T>(T entity) where T : class
        {
            IRepository<T> repository = _unitOfWork.CreateRepository<T>();
            repository.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            IRepository<T> repository = _unitOfWork.CreateRepository<T>();
            repository.Delete(entity);
        }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            IRepository<T> repository = _unitOfWork.CreateRepository<T>();
            return repository.GetAll();
        }
    }
}

using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Prawnbot.Common.Configuration;
using Prawnbot.Core.Log;
using Prawnbot.Core.ServiceLayer;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace Prawnbot.Core.BusinessLayer
{
    public class BaseBl
    {
        // Constructors
        public BaseBl()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("Configuration\\appsettings.json", optional: false);

            ConfigUtility = new ConfigUtility(builder.Build());
        } 

        public static ConfigUtility ConfigUtility;

        protected static ILogging logging;

        // Services
        protected static IAPIService _apiService;
        protected static IBotService _botService;
        protected static ICommandService _commandService;
        protected static IFileService _fileService;
        protected static IDatabaseAccessService _dbAccessService;
        protected static ISpeechRecognitionService _speechRecognitionService;

        protected static IAPIBl _apiBl;
        protected static IBotBl _botBl;
        protected static ICommandBl _commandBl;
        protected static IFileBl _fileBl;
        protected static IDatabaseAccessBl _databaseAccessBl;
        protected static ISpeechRecognitionBl _speechRecognitionBl;

        // Bot properties
        protected readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        protected static DiscordSocketClient _client;
        protected static string _token;
        protected Discord.Commands.CommandService _commands;
        protected IServiceProvider _services;
        protected SocketCommandContext Context;
    }
}

using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Prawnbot.Core.Log
{
    public class CustomLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, CustomLogger> Loggers = new ConcurrentDictionary<string, CustomLogger>();
        private readonly CustomLoggerConfiguration configuration;
        private bool disposedValue;

        public CustomLoggerProvider(CustomLoggerConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return Loggers.GetOrAdd(categoryName, (name) => new CustomLogger(name, configuration));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CustomLoggerProvider()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }
    }
}

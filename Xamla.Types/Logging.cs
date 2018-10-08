using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace Xamla.Types
{
    public static class Logging
    {
        private static ILoggerFactory _loggerFactory;

        public static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_loggerFactory == null)
                {
                    _loggerFactory = new LoggerFactory();
                    _loggerFactory.AddProvider(
                        new DebugLoggerProvider(
                            (string text, LogLevel logLevel) => logLevel > LogLevel.Debug)
                    );
                }
                return _loggerFactory;
            }
            set
            {
                _loggerFactory = value;
            }

        }

        public static ILogger CreateLogger<T>() =>
            LoggerFactory.CreateLogger<T>();

        public static ILogger CreateLogger(string category) =>
            LoggerFactory.CreateLogger(category);
    }
}

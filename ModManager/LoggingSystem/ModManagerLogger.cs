using System;

namespace ModManager.LoggingSystem
{
    public interface IModManagerLogger
    {
        void LogWarning(string message);

        event EventHandler<LoggingEventEventArgs> LoggingEvent;
    }
}

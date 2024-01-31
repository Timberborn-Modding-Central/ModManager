using System;

namespace ModManager.LoggingSystem
{
    public interface IModManagerLogger
    {
        void LogInfo(string message);
        
        void LogWarning(string message);
        
        void LogError(string message);

        event EventHandler<LoggingEventEventArgs> LoggingEvent;
    }
}

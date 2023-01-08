using ModManager.LoggingSystem;
using System;

namespace ModManagerUI
{
    public class ModManagerLogger : IModManagerLogger
    {
        public event EventHandler<LoggingEventEventArgs> LoggingEvent;

        public void LogWarning(string message)
        {
            ModManagerUIPlugin.Log.LogWarning(message);
        }
    }
}

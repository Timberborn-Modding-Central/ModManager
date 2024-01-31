using ModManager.LoggingSystem;
using System;
using ModManager;

namespace ModManagerUI
{
    public class ModManagerLogger : Singleton<ModManagerLogger>, IModManagerLogger
    {
        public event EventHandler<LoggingEventEventArgs> LoggingEvent;
        
        public void LogInfo(string message)
        {
            ModManagerUIPlugin.Log.LogInfo(message);
        }
        
        public void LogWarning(string message)
        {
            ModManagerUIPlugin.Log.LogWarning(message);
        }
        
        public void LogError(string message)
        {
            ModManagerUIPlugin.Log.LogError(message);
        }
    }
}

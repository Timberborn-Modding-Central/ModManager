using System;

namespace ModManager.LoggingSystem
{
    public class LoggingEventEventArgs : EventArgs
    {
        public string Message { get; set; }
        public LoggingLevels LoggingLevel { get; set; }
    }
}

using System;

namespace ModManager
{
    public abstract class Singleton<T> where T : class, new()
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<T> _instance = new Lazy<T>(() => new T());

        public static T Instance => _instance.Value;

    }
}
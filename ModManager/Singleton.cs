namespace ModManager
{
    public abstract class Singleton<T> where T : class, new()
    {
        // ReSharper disable once InconsistentNaming
        private static readonly T? _instance;

        public static T Instance = _instance ??= new T();
    }
}
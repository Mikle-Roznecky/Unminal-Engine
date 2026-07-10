[SupportedOSPlatform("windows")]
public static class Config
{
    private static readonly ConfigManager _manager = new ConfigManager("config.json");
    public static bool IsLoaded { get; private set; } = false;
    public static void Init()
    {
        _manager.Load();
        IsLoaded = true;
    }

    public static T Get<T>(string key, T? defaultValue = default)
    {
        if (!IsLoaded) Init();
        
        var value = _manager.Get(key, defaultValue);

        #pragma warning disable CS8603
        return value ?? defaultValue;
        #pragma warning restore CS8603
    }
    
    public static void Save()
    {
        _manager.Save();
    }
}

namespace Mint.Data;

public class ConfigUtils
{
    public ConfigUtils(string name)
    {
        targetName = name;
    }

    internal string targetName;
    internal Dictionary<Type, dynamic> configs = new();

    /// <summary>
    /// Create config
    /// </summary>
    /// <typeparam name="T">Struct</typeparam>
    /// <returns>Deserialized data</returns>
    public T CreateConfig<T>() where T : struct
    {
        Log.Information("ConfigUtils -> Created config for {Type}", typeof(T).FullName);

        var type = typeof(T);

        var cfg = new Config<T>(targetName);
        cfg.Load();
        cfg.Save();

        if (configs.ContainsKey(type))
            configs.Remove(type);

        configs.Add(type, cfg);

        return cfg.configValue;
    }

    /// <summary>
    /// Get or create config
    /// </summary>
    /// <typeparam name="T">Struct</typeparam>
    /// <returns>Deserialized data</returns>
    public T GetConfig<T>() where T : struct
    {
        var type = typeof(T);

        if (configs.TryGetValue(type, out var cfg))
            return (cfg as Config<T>)?.configValue ?? default;

        return CreateConfig<T>();
    }

    /// <summary>
    /// Update and save config.
    /// </summary>
    /// <typeparam name="T">Struct</typeparam>
    /// <param name="source">New config value</param>
    public void UpdateConfig<T>(T source) where T : struct
    {
        var type = typeof(T);

        if (configs.TryGetValue(type, out var cfg))
        {
            cfg.Settings = source;
            cfg.Save();
        }
    }
    
    /// <summary>
    /// Save config.
    /// </summary>
    /// <typeparam name="T">Struct</typeparam>
    public void SaveConfig<T>() where T : struct
    {
        var type = typeof(T);

        if (configs.TryGetValue(type, out var cfg))
            cfg.Save();
    }

    /// <summary>
    /// Load config.
    /// </summary>
    /// <typeparam name="T">Struct</typeparam>
    public void LoadConfig<T>() where T : struct
    {
        var type = typeof(T);

        if (configs.TryGetValue(type, out var cfg))
            cfg.Load();
    }
}
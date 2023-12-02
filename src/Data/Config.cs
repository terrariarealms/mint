using Newtonsoft.Json;

namespace Mint.Data;

internal class Config<T> where T : struct
{
    internal Config(string name)
    {
        this.name = name;
        path = $"data/{name}.{typeof(T).Name.ToLowerInvariant()}.json";
        configValue = new T();
    }

    internal string name;
    internal string path;
    internal T configValue;

    internal void Load()
    {
        if (File.Exists(path)) configValue = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));

        Save();
    }

    internal void Save()
    {
        var serializedValue = JsonConvert.SerializeObject(configValue, Formatting.Indented);
        File.WriteAllText(path, serializedValue);
    }
}
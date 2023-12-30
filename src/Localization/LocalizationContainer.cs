using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Mint.Localization;

public class LocalizationContainer
{
    public LocalizationContainer()
    {
        _localizedStrings = new Dictionary<string, string>();
    }

    private Dictionary<string, string> _localizedStrings;

    /// <summary>
    /// Import localization from JSON.
    /// </summary>
    /// <param name="json">JSON text</param>
    /// <param name="recursive">Means what if localized string already exists then replace it</param>
    /// <exception cref="InvalidOperationException">Causes when you trying to import from invalid JSON</exception>
    public void ImportFrom(string json, bool recursive, bool fullLocalization)
    {
        Dictionary<string, string>? imported = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        if (imported == null)
            throw new InvalidOperationException("Cannot import localization file.");

        if (fullLocalization)
        {
            _localizedStrings = imported;
            return;
        }

        foreach (KeyValuePair<string, string> local in imported)
            if (!_localizedStrings.ContainsKey(local.Key))
                _localizedStrings.Add(local.Key, local.Value);
            else if (recursive) _localizedStrings[local.Key] = local.Value;
    }

    public string ToJson()
    {
        var text = JsonConvert.SerializeObject(_localizedStrings, Formatting.Indented);
        return text;
    }

    /// <summary>
    /// Translates text.
    /// </summary>
    /// <param name="text">Target text</param>
    /// <returns>Return translated text or null</returns>
    public string? Translate(string text)
    {
        if (_localizedStrings.TryGetValue(text, out var translated) && translated != null)
            return translated;

        return null;
    }
}
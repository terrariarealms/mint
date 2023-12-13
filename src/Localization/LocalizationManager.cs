namespace Mint.Localization;

public sealed class LocalizationManager
{
    public LocalizationManager()
    {
        _containers = new LocalizationContainer?[255];
        _standardLocalization = 0;
    }

    public byte StandardLocalization 
    { 
        get => _standardLocalization;
        set => _standardLocalization = value;
    }

    private byte _standardLocalization;
    private LocalizationContainer?[] _containers;

    /// <summary>
    /// Add container
    /// </summary>
    /// <param name="languageId">Language ID</param>
    /// <param name="container">Localization container</param>
    /// <exception cref="NullReferenceException">Causes when you trying to add nullable localization container</exception>
    public void AddContainer(byte languageId, LocalizationContainer container)
    {
        if (container == null) 
            throw new NullReferenceException($"Cannot add nullable localization container (language id: {languageId})");

        if (_containers[languageId] != null)
            throw new InvalidOperationException($"Cannot replace localization container (language id: {languageId})");

        _containers[languageId] = container;
    }

    /// <summary>
    /// Get localization container
    /// </summary>
    /// <param name="languageId"></param>
    /// <returns></returns>
    public LocalizationContainer? GetContainer(byte languageId) => _containers[languageId];

    /// <summary>
    /// Translate text to other language.
    /// </summary>
    /// <param name="text">Text</param>
    /// <param name="desiredLangId">Desired language id. Can be null if you want to translate to default language</param>
    /// <returns>Translated (or not) text</returns>
    public string Translate(string text, byte? desiredLangId = null)
    {
        if (desiredLangId != null)
        {
            if (desiredLangId == 0) return text;

            string? translated = _containers[desiredLangId.Value]?.Translate(text);
            if (translated != null) return translated;
        }

        return _containers[_standardLocalization]?.Translate(text) ?? text;
    }
}
namespace Mint.Server.Auth;

public struct GroupPresence
{
    public GroupPresence(string? prefix, string? suffix, MintColor? color)
    {
        Prefix = prefix;
        Suffix = suffix;
        Color = color;
    }

    /// <summary>
    /// Safe way to get prefix.
    /// </summary>
    /// <returns>Prefix or blank text (if prefix is null)</returns>
    public string GetPrefix()
    {
        return Prefix + " " ?? "";
    }

    /// <summary>
    /// Safe way to get suffix.
    /// </summary>
    /// <returns>Suffix or blank text (if suffix is null)</returns>
    public string GetSuffix()
    {
        return " " + Suffix ?? "";
    }

    /// <summary>
    /// Safe way to get color.
    /// </summary>
    /// <returns>Color or blank text (if color is null)</returns>
    public MintColor GetColor()
    {
        return Color ?? MintColor.White;
    }

    /// <summary>
    /// Group's prefix. Can be null.
    /// </summary>
    public string? Prefix;

    /// <summary>
    /// Group's suffix. Can be null.
    /// </summary>
    public string? Suffix;

    /// <summary>
    /// Group's color. Can be null.
    /// </summary>
    public MintColor? Color;
}
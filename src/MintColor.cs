using Microsoft.Xna.Framework;

namespace Mint.Core;

// mongodb does not like XNA Color :(
public struct MintColor
{
    public static readonly MintColor White = new MintColor(255, 255, 255);

    public MintColor(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }

    public Color ToXNA() => new Color(R, G, B);

    public byte R;
    public byte G;
    public byte B;
}
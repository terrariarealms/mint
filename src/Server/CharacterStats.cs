using Microsoft.Xna.Framework;

namespace Mint.Server;

public record CharacterStats
{
    public CharacterVisuals Visuals;
    public CharacterDifficulty Difficulty;
    public CharacterExtraFirst ExtraFirst;
    public CharacterExtraSecond ExtraSecond;
}
using MongoDB.Bson.Serialization.Attributes;

namespace Mint.Server.Auth;

[BsonIgnoreExtraElements]
public class CharacterData : DatabaseObject
{
    public CharacterData(string name, PlayerCharacter from) : base(name)
    {
        maxLife = from.MaxLife;
        maxMana = from.MaxMana;
        slots = from.Slots.slots;
        visuals = from.Visuals;
    }

    public int maxLife;
    public int maxMana;
    public NetItem[] slots;
    public PlayerVisuals visuals;
}
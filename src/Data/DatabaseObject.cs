using MongoDB.Bson.Serialization.Attributes;

namespace Mint.Data;

[BsonIgnoreExtraElements]
public abstract class DatabaseObject
{
    public DatabaseObject(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Database object name. Used for finding objects.
    /// </summary>
    public string Name { get; private set; }
}
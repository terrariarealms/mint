namespace Mint.DataStorages;

public abstract class DatabaseObject : IMemoryObject
{
    protected DatabaseObject(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Name of database object.
    /// </summary>
    public string Name { get; protected set; }
}
namespace Mint.DataStorages;

public class MemoryStorage : IObjectStorage<IMemoryObject>
{
    internal Dictionary<string, IMemoryObject> _memObjects = new();

    public string Name => "MemoryStorage";

    public void Initialize()
    {
    }

    /// <summary>
    /// Get memory object.
    /// </summary>
    /// <param name="name">Object name</param>
    /// <returns>IMemoryObject if found</returns>
    public IMemoryObject? Get(string name)
    {
        _memObjects.TryGetValue(name, out var obj);
        return obj;
    }

    /// <summary>
    /// Add/update memory object.
    /// </summary>
    /// <param name="name">Object name</param>
    /// <param name="content">IMemoryObject</param>
    public void Push(string name, IMemoryObject content)
    {
        if (!_memObjects.TryAdd(name, content))
            _memObjects[name] = content;
    }

    /// <summary>
    /// Removes memory object.
    /// </summary>
    /// <param name="name">Object name</param>
    public void Pop(string name)
    {
        _memObjects.Remove(name);
    }
}
namespace Mint.DataStorages;

public interface IObjectStorage<T>
{
    /// <summary>
    /// Storage name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes storage provider.
    /// </summary>
    public void Initialize();

    /// <summary>
    /// Get object from storage.
    /// </summary>
    /// <param name="name">Object name</param>
    /// <returns>Object content or null if object is not exists</returns>
    public T? Get(string name);

    /// <summary>
    /// Adds or updates target object.
    /// </summary>
    /// <param name="name">Object name</param>
    /// <param name="content">Object content</param>
    public void Push(string name, T content);

    /// <summary>
    /// Deletes target object.
    /// </summary>
    /// <param name="name">Objetc name</param>
    /// <param name="content"></param>
    public void Pop(string name);
}
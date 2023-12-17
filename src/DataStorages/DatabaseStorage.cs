using System.Linq.Expressions;
using MongoDB.Driver;

namespace Mint.DataStorages;

public class DatabaseStorage<T> : IObjectStorage<T> where T : DatabaseObject
{
    internal DatabaseStorage(string name)
    {
        Name = name;
        _memCache = new MemoryStorage();
        _collection = MongoDatabase.Database.GetCollection<T>(Name);
    }

    internal MemoryStorage _memCache;
    internal IMongoCollection<T> _collection;

    public string Name { get; }

    public void Initialize()
    {
    }

    /// <summary>
    /// Get object.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public T? Get(string name)
    {
        T? value = _memCache.Get(name) as T;
        if (value != null) return value; 

        var result = _collection.Find((p) => p.Name == name).ToList();
        if (result.Count > 0 && result[0] != null)
        {   
            var obj = result[0];
            _memCache.Push(name, obj);
            return obj;
        }

        return null;
    }


    /// <summary>
    /// Get object all objects.
    /// </summary>
    public List<T> GetAll()
    {
        var list = _collection.Find((p) => true).ToList();
        foreach (T obj in list)
            _memCache.Push(obj.Name, obj);

        return list;
    }

    /// <summary>
    /// Get object by filter.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public T? GetBy(FilterDefinition<T> filter)
    {
        var result = _collection.Find(filter);
        if (result.CountDocuments() > 0)
        {
            var obj = result.First();
            _memCache.Push(obj.Name, obj);

            return obj;
        }

        return null;
    }

    public void Pop(string name)
    {
        _memCache.Pop(name);

        _collection.DeleteOne((p) => p.Name == name);
    }

    /// <summary>
    /// Remove many objects.
    /// </summary>
    /// <param name="contents">Objects enumerable</param>
    public void Pop(IEnumerable<DatabaseObject> contents)
    {
        foreach (DatabaseObject content in contents)
            Pop(content.Name);
    }

    /// <summary>
    /// Remove many objects.
    /// </summary>
    /// <param name="contents">Object names enumerable</param>
    public void Pop(IEnumerable<string> contents)
    {
        foreach (string content in contents)
            Pop(content);
    }

    /// <summary>
    /// Add/update object
    /// </summary>
    /// <param name="name"></param>
    /// <param name="content"></param>
    public void Push(string name, T content)
    {
        if (Get(name) != null)
        {
            _collection.ReplaceOne((p) => p.Name == name, content);
            return;
        }
        
        _collection.InsertOne(content);
        _memCache.Push(name, content);
    }

    /// <summary>
    /// Add/update many objects.
    /// </summary>
    /// <param name="contents">Objects enumerable</param>
    public void Push(IEnumerable<T> contents)
    {
        foreach (T content in contents)
            Push(content.Name, content);
    }
}
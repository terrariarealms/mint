using System.Linq.Expressions;
using MongoDB.Driver;

namespace Mint.Data;

public class DatabaseCollection<T> where T : DatabaseObject
{
    internal DatabaseCollection(string dbName)
    {
        if (MintServer.DatabaseUtils == null)
            throw new InvalidOperationException("Cannot create database collection: database utils not initialized.");

        Name = dbName;
        Db = MintServer.DatabaseUtils.client.GetDatabase(MintServer.Config.Database.Name);
        Cache = new Dictionary<string, T>();
    }

    public string Name { get; private set; }

    internal IMongoDatabase Db;
    internal Dictionary<string, T> Cache;

    /// <summary>
    /// Get all database objects directly (without caching).
    /// </summary>
    /// <returns>Mongo collection with all database objects</returns>
    public IMongoCollection<T> GetCollection()
    {
        if (Db.GetCollection<T>(Name) == null)
            Db.CreateCollection(Name);

        return Db.GetCollection<T>(Name);
    }

    /// <summary>
    /// Add or update database object in cache.
    /// </summary>
    /// <param name="name">Database object name</param>
    /// <param name="cacheValue">Database object value</param>
    public void PushCache(string name, T cacheValue)
    {
        Cache[name] = cacheValue;
    }

    /// <summary>
    /// Remove database object from cache.
    /// </summary>
    /// <param name="name">Database object name</param>
    public void RemoveCache(string name)
    {
        Cache.Remove(name);
    }

    /// <summary>
    /// Find database object in cache.
    /// </summary>
    /// <param name="name">Database object name</param>
    /// <returns>Database object. Null if not found.</returns>
    public T? GetCache(string name)
    {
        if (Cache.TryGetValue(name, out var cache))
            return cache;
        else
            return null;
    }

    /// <summary>
    /// Find database object by name.
    /// </summary>
    /// <param name="name">Database object name</param>
    /// <returns>Database object. Null if not found.</returns>
    public T? Get(string name)
    {
        var cacheValue = GetCache(name);
        if (cacheValue != null)
            return cacheValue;

        var collection = GetCollection();
        var list = collection.Find((p) => p.Name == name).ToList();

        if (list.Count > 0)
        {
            PushCache(name, list[0]);
            return list[0];
        }

        return null;
    }

    /// <summary>
    /// Find database object by filter expression.
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <returns>Database object. Null if not found.</returns>
    public T? Get(Expression<Func<T, bool>> expression)
    {
        var collection = GetCollection();
        var list = collection.Find(expression).ToList();

        if (list.Count > 0)
        {
            return list[0];
        }

        return null;
    }

    /// <summary>
    /// Get all database objects.
    /// </summary>
    /// <returns>List with all database objects</returns>
    public List<T> Get()
    {
        var collection = GetCollection();
        List<T> list = collection.Find((p) => true).ToList();
        foreach (T t in list)
            PushCache(t.Name, t);
        
        return list;
    }

    /// <summary>
    /// Add database object.
    /// </summary>
    /// <param name="dbObject">Object value</param>
    public void Add(T dbObject)
    {
        PushCache(dbObject.Name, dbObject);

        var collection = GetCollection();
        collection.InsertOne(dbObject);
    }

    /// <summary>
    /// Update database object. (Found database object by name and update)
    /// </summary>
    /// <param name="dbObject">Object value</param>
    public void Update(T dbObject)
    {
        PushCache(dbObject.Name, dbObject);

        var collection = GetCollection();
        collection.FindOneAndUpdate((p) => p.Name == dbObject.Name, new ObjectUpdateDefinition<T>(dbObject));
    }

    /// <summary>
    /// Remove database object.
    /// </summary>
    /// <param name="dbObject">Object value</param>
    /// <returns>Is any database object removed</returns>
    public bool Remove(T dbObject) => Remove(dbObject.Name);

    /// <summary>
    /// Remove database object.
    /// </summary>
    /// <param name="name">Name of object</param>
    /// <returns>Is any database object removed</returns>
    public bool Remove(string name)
    {
        var collection = GetCollection();
        return collection.DeleteOne((p) => p.Name == name).DeletedCount > 0;
    }
}
using MongoDB.Driver;

namespace Mint.Data;

public class DatabaseUtils
{
    internal DatabaseUtils()
    {
        databases = new Dictionary<Type, dynamic>();
        
        client = new MongoClient($"mongodb://{MintServer.Config.Database.IP}:{MintServer.Config.Database.Port}");
    }

    internal readonly Dictionary<Type, dynamic> databases;
    internal readonly MongoClient client;

    internal DatabaseCollection<T> InternalCreateDatabase<T>() where T : DatabaseObject
    {
        string name = $"{typeof(T).Name}Collection";
        var database = new DatabaseCollection<T>(name);

        return database;
    }

    /// <summary>
    /// Create database.
    /// </summary>
    /// <typeparam name="T">Database object</typeparam>
    /// <returns></returns>
    public DatabaseCollection<T> CreateDatabase<T>() where T : DatabaseObject
    {
        var database = InternalCreateDatabase<T>();
        databases.TryAdd(typeof(T), database);
        return database;
    }

    /// <summary>
    /// Get or create database.
    /// </summary>
    /// <typeparam name="T">Database object</typeparam>
    /// <returns></returns>
    public DatabaseCollection<T> GetDatabase<T>() where T : DatabaseObject
    {
        var type = typeof(T);

        if (databases.TryGetValue(type, out var database))
            return database;

        return CreateDatabase<T>();
    }
}
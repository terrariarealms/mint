using MongoDB.Driver;

namespace Mint.DataStorages;

public static class MongoDatabase
{
    static MongoDatabase()
    {
        Log.Information("Connecting to MongoDB on {IP}:{Port}", MintServer.Config.Database.IP, MintServer.Config.Database.Port);
        
        string mongoUrl = $"mongodb://{MintServer.Config.Database.IP}:{MintServer.Config.Database.Port}";
        Client = new MongoClient(mongoUrl);
        Log.Information("Using database {Name}", MintServer.Config.Database.Name);
        Database = Client.GetDatabase(MintServer.Config.Database.Name);
    }  

    internal static MongoClient Client;
    internal static IMongoDatabase Database;

    internal static string GetName<T>()
    {
        return $"{typeof(T).Name}Collection";
    }

    /// <summary>
    /// Create database collection instance.
    /// </summary>
    /// <typeparam name="T">Database object type</typeparam>
    /// <returns>Database collection storage</returns>
    public static DatabaseStorage<T> Get<T>() where T : DatabaseObject
    {
        string name = GetName<T>();
        DatabaseStorage<T> storage = new DatabaseStorage<T>(name);
        Log.Information("Created DatabaseStorage for {Name}", name);
        return storage;
    }
}
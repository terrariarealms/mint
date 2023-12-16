using MongoDB.Driver;

namespace Mint.DataStorages;

internal static class MongoDatabase
{
    static MongoDatabase()
    {
        Client = new MongoClient($"mongodb://{MintServer.Config.Database.IP}:{MintServer.Config.Database.Port}");
        Database = Client.GetDatabase(MintServer.Config.Database.Name);

        Collections = new Dictionary<string, dynamic>();
    }  

    internal static MongoClient Client;
    internal static IMongoDatabase Database;
    internal static Dictionary<string, dynamic> Collections;

    internal static string GetName<T>()
    {
        return $"{typeof(T).Name}Collection";
    }

    internal static DatabaseStorage<T> Get<T>() where T : DatabaseObject
    {
        string name = GetName<T>();
        if (Collections.ContainsKey(name))
            return Collections[name];

        IMongoCollection<T> collection = Database.GetCollection<T>(name, null);
        DatabaseStorage<T> storage = new DatabaseStorage<T>(name, collection);

        Collections.TryAdd(name, storage);

        return storage;
    }
}
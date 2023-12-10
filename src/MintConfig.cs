namespace Mint.Core;

public struct MintConfig
{
    public MintConfig()
    {
        Database = new DatabaseConfig();
    }

    public DatabaseConfig Database;
    public GameConfig Game;

    public struct DatabaseConfig
    {
        public DatabaseConfig()
        {
            Name = "MintServerDb";
            IP = "127.0.0.1";
            Port = 27017;
        }

        public string Name;
        public string IP;
        public int Port;
    }

    public struct GameConfig
    {
        public bool UseSSC;
    }
}